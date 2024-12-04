using System.Collections.ObjectModel;
using System.Linq;

namespace freelancer;

public partial class MainPage : ContentPage
{
    private ObservableCollection<Projekt> Projekty = new ObservableCollection<Projekt>();
    private CancellationTokenSource anulowaniePomodoro;

    public MainPage()
    {
        InitializeComponent();
        ListaProjektow.ItemsSource = Projekty;
    }



    private void OnDodajProjektKlikniety(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(ProjektWejscie.Text))
        {
            string priorytet = WybierzPriorytet.SelectedItem?.ToString() ?? "Niski";
            Projekty.Add(new Projekt { Nazwa = ProjektWejscie.Text, Priorytet = priorytet });
            PosortujProjekty();
            ProjektWejscie.Text = string.Empty;
        }
    }



    private void PosortujProjekty()
    {
        var posortowaneProjekty = Projekty.OrderByDescending(p => p.PoziomPriorytetu).ToList();
        Projekty.Clear();
        foreach (var projekt in posortowaneProjekty)
        {
            Projekty.Add(projekt);
        }
    }




    private void OnUsunProjektKlikniety(object sender, EventArgs e)
    {
        if (sender is Button przycisk && przycisk.BindingContext is Projekt projekt)
        {
            Projekty.Remove(projekt);
        }
    }



    private async void OnRozpocznijPomodoroKlikniety(object sender, EventArgs e)
    {
        if (int.TryParse(CzasWejscie.Text, out int minuty) && minuty > 0)
        {
            PomodoroLabel.Text = $"Pomodoro rozpoczęte na {minuty} minut...";
            anulowaniePomodoro?.Cancel();
            anulowaniePomodoro = new CancellationTokenSource();

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(minuty), anulowaniePomodoro.Token);
                PomodoroLabel.Text = "Czas na przerwę!";
            }
            catch (TaskCanceledException)
            {
                PomodoroLabel.Text = "Pomodoro przerwane.";
            }
        }
        else
        {
            await DisplayAlert("Błąd", "Podaj prawidłowy czas w minutach.", "OK");
        }
    }
}

public class Projekt
{
    public string Nazwa { get; set; }
    public string Priorytet { get; set; }
    public int PoziomPriorytetu
    {
        get
        {
            return Priorytet switch
            {
                "Wysoki" => 3,
                "Średni" => 2,
                "Niski" => 1,
                _ => 0
            };
        }
    }
}
