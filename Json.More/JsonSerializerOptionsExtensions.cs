﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Json.More;

/// <summary>
/// Provides extension functionality for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Returns the converter for the specified type.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <param name="typeInfo">An explicit typeInfo to use for looking up the Converter. If not provided, options.GetTypeInfo will be used.</param>
	/// <returns>An implementation of <see cref="JsonConverter{T}"/> as determined by the provided options</returns>
	public static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions options, JsonTypeInfo? typeInfo = null)
	{
#if NET8_0_OR_GREATER
		return (JsonConverter<T>)(typeInfo ?? options.GetTypeInfo(typeof(T))).Converter;
#else
		return (JsonConverter<T>)options.GetConverter(typeof(T));
#endif
	}

	/// <summary>
	/// Read and convert the JSON to T.
	/// </summary>
	/// <remarks>
	/// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
	/// </remarks>
	/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
	/// <param name="typeInfo">An explicit typeInfo to use for looking up the Converter. If not provided, options.GetTypeInfo will be used.</param>
	/// <returns>The value that was converted.</returns>
	public static T? Read<T>(this JsonSerializerOptions options, ref Utf8JsonReader reader, JsonTypeInfo<T>? typeInfo)
	{
#if !NET8_0_OR_GREATER // Workaround for System.Text.Json 6.x missing fix for https://github.com/dotnet/runtime/issues/85172
		if (reader.TokenType == JsonTokenType.Null && typeof(T) == typeof(JsonNode))
			return default;
#endif
		return options.GetConverter<T>(typeInfo).Read(ref reader, typeof(T), options);
	}

	/// <summary>
	/// Write a T to JSON.
	/// </summary>
	/// <remarks>
	/// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
	/// </remarks>
	/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <param name="writer">The <see cref="Utf8JsonReader"/> to read from.</param>
	/// <param name="value">The value to serialize.</param>
	/// <param name="typeInfo">An explicit typeInfo to use for looking up the Converter. If not provided, options.GetTypeInfo will be used.</param>
	/// <returns>The value that was converted.</returns>
	[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Serialize is safe in AOT if the JsonSerializerOptions come from the source generator. Requiring the JsonTypeInfo parameter helps enforce that.")]
	[UnconditionalSuppressMessage("AOT", "IL3050:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Serialize is safe in AOT if the JsonSerializerOptions come from the source generator. Requiring the JsonTypeInfo parameter helps enforce that.")]
	public static void Write<T>(this JsonSerializerOptions options, Utf8JsonWriter writer, T? value, JsonTypeInfo<T>? typeInfo)
	{
		JsonSerializer.Serialize(writer, value, options);
	}

	/// <summary>
	/// Write an object to JSON. If the type is known, prefer Write<![CDATA[<T>]]>
	/// </summary>
	/// <remarks>
	/// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
	/// </remarks>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <param name="writer">The <see cref="Utf8JsonReader"/> to read from.</param>
	/// <param name="value">The value to serialize.</param>
	/// <param name="inputType">The type to serialize.</param>
	/// <returns>The value that was converted.</returns>
	[RequiresDynamicCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	[RequiresUnreferencedCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	public static void Write(this JsonSerializerOptions options, Utf8JsonWriter writer, object? value, Type? inputType)
	{
		if (inputType is not null)
		{
			JsonSerializer.Serialize(writer, value, inputType, options);
		}
		else
		{
			JsonSerializer.Serialize(writer, value, options);
		}
	}
}
