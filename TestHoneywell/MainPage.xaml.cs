namespace TestHoneywell;

public partial class MainPage : ContentPage
{
	int count = 0;
	private string _Barcode;

	public string Barcode
	{
		get { return _Barcode; }
		set { _Barcode = value; OnPropertyChanged(); }
	}

	public MainPage()
	{
		InitializeComponent();
		this.BindingContext = this;
		TestHoneywell.Reader.Read = (e) => { this.Barcode = e; };
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

