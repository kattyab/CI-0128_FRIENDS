using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace UIAutomationTest
{
  public class Case02_Test06_RegisterEmployee
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
    public void CrearEmpleadosEmpresaPI_DeberiaRegistrarEmpleadosCorrectamente()
    {
      _driver.Navigate().GoToUrl("https://localhost:55281/auth/login");

      // Iniciar sesión como administrador
      _driver.FindElement(By.Id("username")).SendKeys("maria@sprint3.cr");
      _driver.FindElement(By.Id("password")).SendKeys("PasswordSeguro123");
      _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

      _wait.Until(d => d.FindElement(By.CssSelector("h1")).Text.Contains("Sprint 3"));

      string[][] empleados = new string[][]
      {
        // name, lastname, birthdate, cedula, bank, email, password, rol, puesto, contrato, ciclo, salario, fechaInicio
        new string[] { "Marcela", "Briseño",    "17-11-1985",   "01-1234-9999", "CR11112222333344445555", "marcela@sprint3.cr","Password1*", "Empleado", "Analista",      "Tiempo Completo", "Mensual", "1750000", "02-05-2025" }
      };

      foreach (var emp in empleados)
      {
        _driver.Navigate().GoToUrl("https://localhost:55281/employees/register");

        System.Threading.Thread.Sleep(1000);

        _driver.FindElement(By.Id("name")).SendKeys(emp[0]);
        _driver.FindElement(By.Id("lastname")).SendKeys(emp[1]);
        _driver.FindElement(By.Id("personid")).SendKeys(emp[3]);

        ClickRadioLabelByForValue("Hombre");
        _driver.FindElement(By.Id("birthdate")).SendKeys(emp[2]);
        _driver.FindElement(By.Id("province")).SendKeys("San José");
        _driver.FindElement(By.Id("canton")).SendKeys("Montes de Oca");
        _driver.FindElement(By.Id("othersigns")).SendKeys("Cerca de la empresa");
        _driver.FindElement(By.Id("phonenumber")).SendKeys("8888-8888");

        ClickRadioLabelByForValue(emp[7]);
        _driver.FindElement(By.Id("jobposition")).SendKeys(emp[8]);
        ClickRadioLabelByForValue(emp[9]);
        ClickRadioLabelByForValue(emp[10]);

        _driver.FindElement(By.Id("brutesalary")).SendKeys(emp[11]);
        _driver.FindElement(By.Id("startdate")).SendKeys(emp[12]);
        _driver.FindElement(By.Id("bankaccount")).SendKeys(emp[4]);
        _driver.FindElement(By.Id("email")).SendKeys(emp[5]);
        _driver.FindElement(By.Id("password")).SendKeys(emp[6]);

        var submitBtn = _driver.FindElement(By.CssSelector("form button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

        _wait.Until(d => d.FindElement(By.CssSelector(".modal-content")));
        var modalBtn = _driver.FindElement(By.CssSelector(".modal button[type='button']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", modalBtn);
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", modalBtn);
      }
    }

    private void ClickRadioLabelByForValue(string forValue)
    {
      var label = _wait.Until(driver =>
      {
        try
        {
          var el = driver.FindElement(By.CssSelector($"label[for='{forValue}']"));
          return (el.Displayed && el.Enabled) ? el : null;
        }
        catch (StaleElementReferenceException) { return null; }
        catch (NoSuchElementException) { return null; }
      });

      ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", label);
      ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", label);
    }

    [TearDown]
    public void TearDown()
    {
      _driver.Quit();
      _driver.Dispose();
    }
  }
}
