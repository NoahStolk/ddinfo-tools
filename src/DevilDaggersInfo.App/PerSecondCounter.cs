namespace DevilDaggersInfo.App;

public class PerSecondCounter
{
	private int _currentSecond;

	private int _currentCount;

	public int CountPerSecond { get; private set; }

	public void Increment()
	{
		int currentSecond = DateTime.Now.Second;
		if (currentSecond != _currentSecond)
		{
			_currentSecond = currentSecond;
			CountPerSecond = _currentCount;
			_currentCount = 0;
		}
		else
		{
			_currentCount++;
		}
	}
}
