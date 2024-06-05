namespace DevilDaggersInfo.Tools.Networking;

public record ApiResult<TResult>
	where TResult : class
{
	private readonly bool _success;
	private readonly TResult? _value;
	private readonly ApiError? _error;

	private ApiResult(TResult? v, ApiError? e, bool success)
	{
		_value = v;
		_error = e;
		_success = success;
	}

	public static ApiResult<TResult> Ok(TResult result)
	{
		return new ApiResult<TResult>(result, default, true);
	}

	public static ApiResult<TResult> Error(ApiError error)
	{
		return new ApiResult<TResult>(default, error, false);
	}

	public void Match(Action<TResult> onSuccess, Action<ApiError> onError)
	{
		if (_success)
		{
			if (_value == null)
				throw new InvalidOperationException("Bad internal ApiResult: Value is null but success is true.");

			onSuccess(_value);
		}

		if (_error == null)
			throw new InvalidOperationException("Bad internal ApiResult: Error is null but success is false.");

		onError(_error);
	}
}
