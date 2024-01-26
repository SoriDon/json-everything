﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Json.More
{
	/// <summary>
	/// Interface to enable JsonConverter implementations to call other JsonConverter's Read methods
	/// without statically being aware of their type parameters.
	/// </summary>
	public interface IJsonConverterReadWrite
	{
#if NET8_0_OR_GREATER
		/// <summary>Reads and converts the JSON to object?.</summary>
		/// <param name="reader">The reader.</param>
		/// <param name="typeToConvert">The type to convert.</param>
		/// <param name="options">An object that specifies serialization options to use.</param>
		/// <returns>The converted value.</returns>
		object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

		/// <summary>Writes a specified value as JSON.</summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="value">The value to convert to JSON.</param>
		/// <param name="options">An object that specifies serialization options to use.</param>
		void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options);
#endif
	}

	/// <summary>
	/// Interface to enable JsonConverter implementations to call other JsonConverter's Read methods
	/// without statically being aware of their type parameters.
	/// </summary>
	public interface IJsonConverterReadWrite<T> : IJsonConverterReadWrite
	{
#if NET8_0_OR_GREATER      
		/// <summary>Reads and converts the JSON to object?.</summary>                                                                     
		/// <param name="reader">The reader.</param>
		/// <param name="typeToConvert">The type to convert.</param>
		/// <param name="options">An object that specifies serialization options to use.</param>
		/// <returns>The converted value.</returns>
		new T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

		/// <summary>Writes a specified value as JSON.</summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="value">The value to convert to JSON.</param>
		/// <param name="options">An object that specifies serialization options to use.</param>
		void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

		object? IJsonConverterReadWrite.Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return Read(ref reader, typeToConvert, options);
		}

		void IJsonConverterReadWrite.Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			Write(writer, (T)value, options);
		}
#endif
	}

}
