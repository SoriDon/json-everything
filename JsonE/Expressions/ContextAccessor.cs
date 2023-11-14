﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace Json.JsonE.Expressions;

internal class ContextAccessor
{
	private readonly IContextAccessorSegment[] _segments;

	public static ContextAccessor Now { get; } = new(new[] { new PropertySegment("now", false) });
	public static ContextAccessor Root { get; } = new(Array.Empty<PropertySegment>());

	private ContextAccessor(IEnumerable<IContextAccessorSegment> segments)
	{
		_segments = segments.ToArray();
	}

	public static bool TryParse(ReadOnlySpan<char> source, ref int index, out ContextAccessor? accessor)
	{
		int i = index;
		if (!source.ConsumeWhitespace(ref i))
		{
			accessor = null;
			return false;
		}

		if (!source.TryParseName(ref i, out var name))
		{
			accessor = null;
			return false;
		}

		if (name.In("true", "false", "null"))
		{
			accessor = null;
			return false;
		}

		var segments = new List<IContextAccessorSegment>{new PropertySegment(name!, false)};

		while (i < source.Length)
		{
			if (!source.ConsumeWhitespace(ref i))
			{
				accessor = null;
				return false;
			}

			switch (source[i])
			{
				case '.':
					i++;
					if (!source.TryParseName(ref i, out name))
					{
						accessor = null;
						return false;
					}

					segments.Add(new PropertySegment(name!, false));
					continue;
				case '[':
					i++;

					if (!source.ConsumeWhitespace(ref i))
					{
						accessor = null;
						return false;
					}

					if (!TryParseQuotedName(source, ref i, out var segment) &&
					    !TryParseSlice(source, ref i, out segment) &&
					    !TryParseIndex(source, ref i, out segment))
					{
						accessor = null;
						return false;
					}

					segments.Add(segment!);

					if (!source.ConsumeWhitespace(ref i))
					{
						accessor = null;
						return false;
					}

					if (source[i] != ']')
					{
						accessor = null;
						return false;
					}

					i++;

					continue;
			}

			break;
		}

		index = i;
		accessor = new ContextAccessor(segments);
		return true;
	}

	public static ContextAccessor ParseStub(string source, string expectedVariable)
	{
		int index = 0;
		if (!TryParse(source.AsSpan(), ref index, out var accessor)) throw new TypeException("source.slice is not a function");

		var prop = (PropertySegment)accessor!._segments[0];
		if (prop.Name != expectedVariable)
			throw new InterpreterException($"unknown context value {prop.Name}");

		return new ContextAccessor(accessor!._segments.Skip(1));
	}

	private static bool TryParseQuotedName(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		char quoteChar;
		var i = index;
		switch (source[index])
		{
			case '"':
				quoteChar = '"';
				i++;
				break;
			case '\'':
				quoteChar = '\'';
				i++;
				break;
			default:
				segment = null;
				return false;
		}

		var done = false;
		var sb = new StringBuilder();
		while (i < source.Length && !done)
		{
			if (source[i] == quoteChar)
			{
				done = true;
				i++;
			}
			else
			{
				if (!source.EnsureValidNameCharacter(i))
				{
					segment = null;
					return false;
				}
				sb.Append(source[i]);
				i++;
			}
		}

		if (!done)
		{
			segment = null;
			return false;
		}

		index = i;
		segment = new PropertySegment(sb.ToString(), true);
		return true;

	}

	private static bool TryParseIndex(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		if (!source.TryGetInt(ref index, out var i))
		{
			segment = null;
			return false;
		}

		segment = new IndexSegment(i);
		return true;
	}

	private static bool TryParseSlice(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		var i = index;
		int? start = null, end = null, step = null;

		if (source.TryGetInt(ref i, out var value))
			start = value;

		if (!source.ConsumeWhitespace(ref i))
		{
			segment = null;
			return false;
		}

		if (source[i] != ':')
		{
			segment = null;
			return false;
		}

		i++; // consume :

		if (!source.ConsumeWhitespace(ref i))
		{
			segment = null;
			return false;
		}

		if (source.TryGetInt(ref i, out value))
			end = value;

		if (!source.ConsumeWhitespace(ref i))
		{
			segment = null;
			return false;
		}

		if (source[i] == ':')
		{
			i++; // consume :

			if (!source.ConsumeWhitespace(ref i))
			{
				segment = null;
				return false;
			}

			if (source.TryGetInt(ref i, out value))
				step = value;
		}

		index = i;
		segment = new SliceSegment(start, end, step);
		return true;
	}

	public bool TryFind(JsonNode? target, out JsonNode? value)
	{
		var current = target;
		foreach (var segment in _segments)
		{
			if (!segment.TryFind(current, out value)) return false;

			current = value;
		}

		value = current;
		return true;
	}
}