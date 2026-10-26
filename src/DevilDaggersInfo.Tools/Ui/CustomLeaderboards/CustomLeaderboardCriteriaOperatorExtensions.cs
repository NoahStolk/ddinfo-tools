using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal static class CustomLeaderboardCriteriaOperatorExtensions
{
	extension(CustomLeaderboardCriteriaOperator @operator)
	{
		public Core.CriteriaExpression.CustomLeaderboardCriteriaOperator ToCore()
		{
			return @operator switch
			{
				CustomLeaderboardCriteriaOperator.Any => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Any,
				CustomLeaderboardCriteriaOperator.Equal => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Equal,
				CustomLeaderboardCriteriaOperator.LessThan => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThan,
				CustomLeaderboardCriteriaOperator.GreaterThan => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThan,
				CustomLeaderboardCriteriaOperator.LessThanOrEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThanOrEqual,
				CustomLeaderboardCriteriaOperator.GreaterThanOrEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThanOrEqual,
				CustomLeaderboardCriteriaOperator.Modulo => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Modulo,
				CustomLeaderboardCriteriaOperator.NotEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.NotEqual,
				_ => throw new UnreachableException(),
			};
		}
	}
}
