﻿using System.Text.Json.Nodes;
using Json.More;

namespace Json.JsonE.Expressions;

internal class PropertySegment : IContextAccessorSegment
{
	private readonly bool _isBracketed;

	public string Name { get; }

	public PropertySegment(string name, bool isBracketed)
	{
		Name = name;
		_isBracketed = isBracketed;
	}

	public bool TryFind(JsonNode? target, out JsonNode? value)
	{
		value = null;
		if (target is JsonObject obj) return obj.TryGetValue(Name, out value, out _);
		
		if (!_isBracketed) throw new InterpreterException("infix: . expects objects");
		
		return false;

	}
}