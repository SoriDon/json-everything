﻿using System.Text.Json.Nodes;
using Json.More;

namespace Json.JsonE.Expressions;

internal class SubtractOperator : IBinaryOperator
{
	public int Precedence => 7;

	public JsonNode? Evaluate(JsonNode? left, JsonNode? right)
	{
		if (left is not JsonValue lValue ||
		    right is not JsonValue rValue)
			throw new InterpreterException("infix: - expects number - number");

		var lNumber = lValue.GetNumber() ?? throw new InterpreterException("infix: - expects number - number");
		var rNumber = rValue.GetNumber() ?? throw new InterpreterException("infix: - expects number - number");

		return lNumber - rNumber;
	}

	public override string ToString()
	{
		return "-";
	}
}