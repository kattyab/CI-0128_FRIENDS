using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace UIAutomationTest
{
  public class Case02_Test06_SuscribeBenefits
  {
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [SetUp]
    public void Setup()
    {
      _driver = new ChromeDriver();
      _driver.Manage().Window.Maximize();
      _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [Test]
    public void SuscribirBeneficiosPorEmpleado()
    {
      SuscribirBeneficioEmpleado(
          "marcela@sprint3.cr", "Password1*",
          new (string beneficio, string dependientes)[] {
            ("Salario escolar", null),
            ("Asociación solidarista", "Sprint 3")
          }
      );
    }

    private void SuscribirBeneficioEmpleado(string email, string password, (string beneficio, string dependientes)[] beneficios)
    {
      _driver.Navigate().GoToUrl("https://localhost:55281/auth/login");
      _wait.Until(d => d.FindElement(By.Id("username")));
      _driver.FindElement(By.Id("username")).Clear();
      _driver.FindElement(By.Id("username")).SendKeys(email);
      _driver.FindElement(By.Id("password")).Clear();
      _driver.FindElement(By.Id("password")).SendKeys(password);
      _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
      _wait.Until(d => d.Url.Contains("/dashboardemployee"));

      _driver.Navigate().GoToUrl("https://localhost:55281/benefits/subscribe");
      _wait.Until(d => d.FindElement(By.TagName("h1")).Text.Contains("Beneficios"));

      foreach (var (beneficio, dependientes) in beneficios)
      {
        var suscribirBtn = _driver.FindElement(By.XPath("//button[contains(.,'Suscribir beneficio')]"));
        suscribirBtn.Click();

                System.Threading.Thread.Sleep(200);

                _wait.Until(d => d.FindElements(By.CssSelector("table.table-hover tbody tr")).Any());

        var filas = _driver.FindElements(By.CssSelector("table.table-hover tbody tr"));
        var fila = filas.FirstOrDefault(f => f.Text.Contains(beneficio));
        Assert.IsNotNull(fila, $"No se encontró el beneficio '{beneficio}' para el usuario {email}");

        var radio = fila.FindElement(By.CssSelector("input[type='radio']"));
        radio.Click();

        var continuarBtn = _driver.FindElement(By.XPath("//button[contains(.,'Continuar')]"));
        continuarBtn.Click();

        _wait.Until(d => d.FindElement(By.CssSelector(".modal-content .alert-info")));

        if (dependientes != null && beneficio.ToLower().Contains("seguro"))
        {
          var dependentsInput = _driver.FindElement(By.Id("dependents"));
          dependentsInput.Clear();
          dependentsInput.SendKeys(dependientes);
        }

                if (dependientes != null && beneficio.ToLower().Contains("solidarista"))
                {
                    var dependentsInput = _driver.FindElement(By.Id("assocName"));
                    dependentsInput.Clear();
                    dependentsInput.SendKeys(dependientes);
                }

                var confirmarBtn = _driver.FindElement(By.XPath("//button[contains(.,'Confirmar Suscripción')]"));
        confirmarBtn.Click();

        _wait.Until(d =>
        {
          var alert = d.FindElement(By.CssSelector(".alert-success"));
          return !string.IsNullOrWhiteSpace(alert.Text);
        });
        var successMsg = _driver.FindElement(By.CssSelector(".alert-success")).Text;
        Console.WriteLine("Mensaje de éxito: " + successMsg);
        StringAssert.Contains("Se ha suscrito exitosamente al beneficio", successMsg);

        var aceptarBtn = _driver.FindElement(By.XPath("//button[contains(.,'Aceptar')]"));
        aceptarBtn.Click();

        System.Threading.Thread.Sleep(1000);
      }
    }

    [TearDown]
    public void TearDown()
    {
      _driver.Quit();
      _driver.Dispose();
    }
  }
}