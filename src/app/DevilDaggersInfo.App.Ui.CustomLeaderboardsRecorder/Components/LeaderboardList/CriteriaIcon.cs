using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Extensions;
using DevilDaggersInfo.Core.CriteriaExpression;
using DevilDaggersInfo.Core.CriteriaExpression.Parts;
using DevilDaggersInfo.Types.Core.CustomLeaderboards.Extensions;
using System.Text;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class CriteriaIcon : TooltipSprite
{
	public CriteriaIcon(IBounds bounds, GetCustomLeaderboardCriteria criteria)
		: base(bounds, criteria.Type.GetTexture(), criteria.Type.GetColor(), GetText(criteria), TextAlign.Left)
	{
	}

	private static string GetText(GetCustomLeaderboardCriteria criteria)
	{
		if (!Expression.TryParse(criteria.Expression, out Expression? criteriaExpression))
		{
			// TODO: Log warning.
			return string.Empty;
		}

		Types.Core.CustomLeaderboards.CustomLeaderboardCriteriaType criteriaType = criteria.Type.ToCore();
		Types.Core.CustomLeaderboards.CustomLeaderboardCriteriaOperator @operator = criteria.Operator.ToCore();

		StringBuilder sb = new();
		sb.Append(criteriaType.Display());
		sb.Append(' ');
		sb.Append(@operator.ShortString());
		sb.Append(' ');

		foreach (IExpressionPart expressionPart in criteriaExpression.Parts)
		{
			switch (expressionPart)
			{
				case ExpressionOperator op:
					sb.Append(op);
					break;
				case ExpressionTarget target:
					sb.Append(target);
					break;
				case ExpressionValue value:
					sb.Append(value.ToDisplayString(criteriaType));
					break;
			}

			sb.Append(' ');
		}

		return sb.ToString().Trim();
	}
}
