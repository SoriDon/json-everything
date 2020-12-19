﻿using System.Text.Json;
using Json.More;

namespace Json.Logic.Components
{
	[Operator("!==")]
	internal class StrictNotEqualsComponent : LogicComponent
	{
		private readonly LogicComponent _a;
		private readonly LogicComponent _b;

		public StrictNotEqualsComponent(LogicComponent a, LogicComponent b)
		{
			_a = a;
			_b = b;
		}

		public override JsonElement Apply(JsonElement data)
		{
			return (!_a.Apply(data).IsEquivalentTo(_b.Apply(data))).AsJsonElement();
		}
	}
}