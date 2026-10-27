using Reqnroll;

namespace DummyJson.ApiTests.Support;

[Binding]
public sealed class Hooks(ScenarioContext scenarioContext, HttpLog httpLog, IReqnrollOutputHelper output)
{
    [AfterScenario]
    public void WriteHttpDetailsForFailedScenario()
    {
        if (scenarioContext.TestError is null || httpLog.Entries.Count == 0)
            return;

        output.WriteLine("");
        output.WriteLine($"===== HTTP details for failed scenario: {scenarioContext.ScenarioInfo.Title} =====");
        foreach (var entry in httpLog.Entries)
            output.WriteLine(entry);
    }
}
