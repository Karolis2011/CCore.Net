using CCore.Net.JsRt;
using CCore.Net.Managed;
using CCore.Net.Runtimes;
using Spectre.Console;
using System;


using var runtime = new BasicJsRuntime(CCore.Net.JsRuntimeAttributes.None);
using var scope = new BasicJsRuntime.Scope(runtime);


var helper = new ReportingHelper();
var obj = new ClassToMap();

JsObject.GlobalObject["obj"] = JsManagedObject.Obtain(obj);
JsObject.GlobalObject["helper"] = JsManagedObject.Obtain(helper);

JsContext.RunScript(@"
function SafePropertyGet(object, property) {
  try {
    return object[property]
  } catch (e) {
    return `<${e}>`
  }
}
function FullReport(name, value) {
  helper.SetTitle(name)
  for (var name in value) {
    helper.Report(name, value.hasOwnProperty(name), SafePropertyGet(value, name))
  }
  helper.FinishReporting()
}

FullReport('obj', obj)
FullReport('helper', helper)
");

public class ReportingHelper
{
    public Table AnsiTable;

    public ReportingHelper() => Initialize();
    private void Initialize()
    {
        AnsiTable = new Table();
        AnsiTable.AddColumns("Name", "isOwn", "Value");
    }

    public void SetTitle(string title) => AnsiTable.Title = new TableTitle(title);
    public void Report(string name, JsBool own, JsValue value) =>
        AnsiTable.AddRow(new Text(name), new Text(own.ToString()), new Text(value.ConvertToString()));

    public void FinishReporting()
    {
        AnsiConsole.Write(AnsiTable);
        Initialize();
    }
}

public class ClassToMap
{
    public int Test { get; set; }
    public ClassToMap This => this;
}