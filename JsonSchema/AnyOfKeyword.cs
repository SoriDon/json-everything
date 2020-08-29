﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace Json.Schema
{
	/// <summary>
	/// Handles `anyOf`.
	/// </summary>
	[Applicator]
	[SchemaPriority(20)]
	[SchemaKeyword(Name)]
	[SchemaDraft(Draft.Draft6)]
	[SchemaDraft(Draft.Draft7)]
	[SchemaDraft(Draft.Draft201909)]
	[Vocabulary(Vocabularies.Applicator201909Id)]
	[JsonConverter(typeof(AnyOfKeywordJsonConverter))]
	public class AnyOfKeyword : IJsonSchemaKeyword, IRefResolvable
	{
		internal const string Name = "anyOf";

		/// <summary>
		/// The keywords schema collection.
		/// </summary>
		public IReadOnlyList<JsonSchema> Schemas { get; }

		/// <summary>
		/// Creates a new <see cref="AnyOfKeyword"/>.
		/// </summary>
		/// <param name="values">The set of schemas.</param>
		public AnyOfKeyword(params JsonSchema[] values)
		{
			Schemas = values.ToList();
		}

		/// <summary>
		/// Creates a new <see cref="AnyOfKeyword"/>.
		/// </summary>
		/// <param name="values">The set of schemas.</param>
		public AnyOfKeyword(IEnumerable<JsonSchema> values)
		{
			Schemas = values.ToList();
		}

		/// <summary>
		/// Provides validation for the keyword.
		/// </summary>
		/// <param name="context">Contextual details for the validation process.</param>
		public void Validate(ValidationContext context)
		{
			var overallResult = false;
			for (var i = 0; i < Schemas.Count; i++)
			{
				var schema = Schemas[i];
				var subContext = ValidationContext.From(context, subschemaLocation: context.SchemaLocation.Combine(PointerSegment.Create($"{i}")));
				schema.ValidateSubschema(subContext);
				overallResult |= subContext.IsValid;
				context.NestedContexts.Add(subContext);
			}

			if (overallResult)
				context.ConsolidateAnnotations();
			context.IsValid = overallResult;
		}

		IRefResolvable IRefResolvable.ResolvePointerSegment(string value)
		{
			if (!int.TryParse(value, out var index)) return null;
			if (index < 0 || Schemas.Count <= index) return null;

			return Schemas[index];
		}

		void IRefResolvable.RegisterSubschemas(SchemaRegistry registry, Uri currentUri)
		{
			foreach (var schema in Schemas)
			{
				schema.RegisterSubschemas(registry, currentUri);
			}
		}
	}

	internal class AnyOfKeywordJsonConverter : JsonConverter<AnyOfKeyword>
	{
		public override AnyOfKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartArray)
			{
				var schemas = JsonSerializer.Deserialize<List<JsonSchema>>(ref reader, options);
				return new AnyOfKeyword(schemas);
			}
			
			var schema = JsonSerializer.Deserialize<JsonSchema>(ref reader, options);
			return new AnyOfKeyword(schema);
		}
		public override void Write(Utf8JsonWriter writer, AnyOfKeyword value, JsonSerializerOptions options)
		{
			writer.WritePropertyName(AnyOfKeyword.Name);
			writer.WriteStartArray();
			foreach (var schema in value.Schemas)
			{
				JsonSerializer.Serialize(writer, schema, options);
			}
			writer.WriteEndArray();
		}
	}
}