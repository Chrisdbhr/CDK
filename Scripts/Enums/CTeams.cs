using System;

namespace CDK {
	public enum Teams {
		NEUTRAL_GAYA = 0,
		PLAYER = 1,
		ALLY = 2,
		ENEMY = 3
	}
	
	public static class CTeamsExtensions {
		public static bool IsEnemy(this Teams team, Teams other) {
			switch (team) {
				case Teams.NEUTRAL_GAYA:
					return false;
				case Teams.PLAYER:
					return other == Teams.ENEMY;
				case Teams.ALLY:
					return other == Teams.ENEMY;
				case Teams.ENEMY:
					return other == Teams.PLAYER || other == Teams.ALLY;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
