using OWML.Common;
using OWML.ModHelper;

using OnAirTap;

namespace OnAirTap_OWML;

public class OnAirTap_OWML : ModBehaviour
{
	public static OnAirTap_OWML instance;

	OnAirTap.Plugin plug;

	public void Awake()
	{
		instance = this;
	}

	public void BindConfigs(){}
	public void ValidateConfigs(){}
	public void SendConfigs(){}

	public void Start()
	{
		Plugin.logger = new OWLogger(ModHelper.Console);


		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"My mod {nameof(OnAirTap_OWML)} is loaded!", MessageType.Success);
	}

	void Update(){}
	void LateUpdate(){}

}

