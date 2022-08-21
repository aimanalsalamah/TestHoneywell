using Android.App;
using Android.Content.PM;
using Android.OS;
using Honeywell.AIDC.CrossPlatform;

namespace TestHoneywell;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public MainActivity()
    {
        var s = new Scanner();
        s.Init(this);
    }
}

public class Scanner
{
    private BarcodeReader SelectedReader { get; set; }
    public async void Init(object obj)
    {
        var scanersList = await BarcodeReader.GetConnectedBarcodeReaders(obj);
        SelectedReader = new BarcodeReader(scanersList.FirstOrDefault().ScannerName, obj);
        //SelectedReader.BarcodeDataReady += (object sender, BarcodeDataArgs e) => action(e.Data);
        SelectedReader.BarcodeDataReady += SelectedReader_BarcodeDataReady;
        OpenBarcodeReader();
    }

    private void SelectedReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
    {
        try
        {
            var data = e.Data;
            //this.ScanAction(e.Data);
            //Send data to Maui
        }
        catch (Exception ex)
        {
        }
    }
    public async void SetScannerAndSymbologySettings()
    {
        try
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
                if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                {
                    throw new Exception();
                    //DisplayAlert("Error", "Symbology settings failed, Code:" + result.Code +
                    //                    " Message:" + result.Message, context);
                }
            }

        }
        catch (Exception)
        {
            throw;
            //DisplayAlert("Error", "Symbology settings failed. Message: " + exp.Message, context);
        }

    }
    public async void OpenBarcodeReader()
    {
        if (!SelectedReader.IsReaderOpened)
        {
            BarcodeReader.Result result = await SelectedReader.OpenAsync();

            if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
            {
                SetScannerAndSymbologySettings();
            }
            else
            {
                //throw new Exception();
                //await DisplayAlert("Error", "OpenAsync failed, Code:" + result.Code +
                //    " Message:" + result.Message, "OK");
            }
        }
    }
}
