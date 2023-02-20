using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.Settings.Model;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Layouts;

public class SettingsLayout : Layout, IExtendedLayout
{
	public SettingsLayout()
	{
		const int headerHeight = 24;
		const int labelX = 256;
		const int settingX = 512;

		CheckboxStyle checkboxStyle = new(8, 4, 6);

		MainLayoutBackButton backButton = new(new PixelBounds(0, 0, 24, headerHeight), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.MainLayout)));
		NestingContext.Add(backButton);

		int y = 128;
		Checkbox scaleUiToWindowCheckbox = AddCheckbox("Scale UI to window", OnChangeScaleUiToWindow);
		Checkbox showDebugOutputCheckbox = AddCheckbox("Show debug output", b => UserSettings.Model = UserSettings.Model with { ShowDebugOutput = b });
		Checkbox renderWhileWindowIsInactiveCheckbox = AddCheckbox("Render while window is inactive", b => UserSettings.Model = UserSettings.Model with { RenderWhileWindowIsInactive = b });
		Slider maxFpsSlider = AddSlider("Max FPS", f => UserSettings.Model = UserSettings.Model with { MaxFps = (int)f }, false, UserSettingsModel.MaxFpsMin, UserSettingsModel.MaxFpsMax, 1, UserSettings.Model.MaxFps, SliderStyles.Default with { ValueFormat = "0" });
		Slider lookSpeedSlider = AddSlider("Look speed", f => UserSettings.Model = UserSettings.Model with { LookSpeed = f }, false, UserSettingsModel.LookSpeedMin, UserSettingsModel.LookSpeedMax, 0.05f, UserSettings.Model.LookSpeed, SliderStyles.Default);
		Slider fieldOfViewSlider = AddSlider("Field of view", f => UserSettings.Model = UserSettings.Model with { FieldOfView = (int)f }, false, UserSettingsModel.FieldOfViewMin, UserSettingsModel.FieldOfViewMax, 1, UserSettings.Model.FieldOfView, SliderStyles.Default with { ValueFormat = "0" });

		Checkbox AddCheckbox(string label, Action<bool> onClick)
		{
			Checkbox checkbox = new(new PixelBounds(settingX, y, 24, 24), onClick, checkboxStyle);
			AddSetting(label, checkbox);
			return checkbox;
		}

		Slider AddSlider(string label, Action<float> onChange, bool showValue, float min, float max, float step, float defaultValue, SliderStyle sliderStyle)
		{
			Slider slider = new(new PixelBounds(settingX, y, 256, 24), onChange, showValue, min, max, step, defaultValue, sliderStyle);
			AddSetting(label, slider);
			return slider;
		}

		void AddSetting(string label, AbstractComponent component)
		{
			NestingContext.Add(new Label(new PixelBounds(labelX, y, 256, 32), label, LabelStyles.DefaultLeft));
			NestingContext.Add(component);
			y += 32;
		}

		StateManager.Subscribe<UserSettingsLoaded>(OnUserSettingsLoaded);

		void OnUserSettingsLoaded()
		{
			scaleUiToWindowCheckbox.CurrentValue = UserSettings.Model.ScaleUiToWindow;
			showDebugOutputCheckbox.CurrentValue = UserSettings.Model.ShowDebugOutput;
			renderWhileWindowIsInactiveCheckbox.CurrentValue = UserSettings.Model.RenderWhileWindowIsInactive;
			maxFpsSlider.CurrentValue = UserSettings.Model.MaxFps;
			lookSpeedSlider.CurrentValue = UserSettings.Model.LookSpeed;
			fieldOfViewSlider.CurrentValue = UserSettings.Model.FieldOfView;
		}
	}

	private static void OnChangeScaleUiToWindow(bool value)
	{
		UserSettings.Model = UserSettings.Model with { ScaleUiToWindow = value };
		ViewportState.UpdateViewports(CurrentWindowState.Width, CurrentWindowState.Height);
	}

	public void Update()
	{
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		Vector2i<int> windowScale = new(CurrentWindowState.Width, CurrentWindowState.Height);
		Game.Self.RectangleRenderer.Schedule(windowScale, windowScale / 2, -100, Color.Gray(0.1f));

		Game.Self.MonoSpaceFontRenderer32.Schedule(Vector2i<int>.One, new(512, 64), 0, Color.White, "Settings", TextAlign.Middle);
	}
}
