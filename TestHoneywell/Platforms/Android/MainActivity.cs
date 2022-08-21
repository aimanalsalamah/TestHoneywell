namespace TestHoneywell;
using Android.App;
using Android.Content.PM;
using Honeywell.AIDC.CrossPlatform;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public MainActivity()
    {
        new Scanner().Init(this).ConfigureAwait(false);
    }
}

public class Scanner
{
    private BarcodeReader SelectedReader { get; set; }
    public async Task Init(object obj)
    {
        var scanersList = await BarcodeReader.GetConnectedBarcodeReaders(obj);
        SelectedReader = new BarcodeReader(scanersList.FirstOrDefault().ScannerName, obj);
        SelectedReader.BarcodeDataReady += SelectedReader_BarcodeDataReady;
        await OpenBarcodeReader();
    }

    private void SelectedReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
    {
        TestHoneywell.Reader.Read(e.Data);
    }
    public async Task SetScannerAndSymbologySettings()
    {
        if (SelectedReader.IsReaderOpened)
        {
            Dictionary<string, object> settings = new Dictionary<string, object>()
                {
                    {SelectedReader.SettingKeys.TriggerScanMode, SelectedReader.SettingValues.TriggerScanMode_OneShot },
                    {SelectedReader.SettingKeys.Code128Enabled, true },
                    {SelectedReader.SettingKeys.Code39Enabled, true },
                    {SelectedReader.SettingKeys.Ean8Enabled, true },
                    {SelectedReader.SettingKeys.Ean8CheckDigitTransmitEnabled, true },
                    {SelectedReader.SettingKeys.Ean13Enabled, true },
                    {SelectedReader.SettingKeys.Ean13CheckDigitTransmitEnabled, true },
                    {SelectedReader.SettingKeys.Interleaved25Enabled, true },
                    {SelectedReader.SettingKeys.Interleaved25MaximumLength, 100 },
                    {SelectedReader.SettingKeys.Postal2DMode, SelectedReader.SettingValues.Postal2DMode_Usps }
                };

            BarcodeReader.Result result = await SelectedReader.SetAsync(settings);
        }
    }
    public async Task OpenBarcodeReader()
    {
        if (!SelectedReader.IsReaderOpened)
        {
            BarcodeReader.Result result = await SelectedReader.OpenAsync();

            if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
            {
                await SetScannerAndSymbologySettings();
            }
            else
            {
                throw new Exception( "OpenAsync failed, Code:" + result.Code +
                    " Message:" + result.Message);
            }
        }
    }
}
