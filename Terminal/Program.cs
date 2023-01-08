// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using PuppeteerSharp;
using Scout.Core.Builders;
using Scout.Core.Commands;
using Scout.Domain;
using Scout.Queries;

Console.WriteLine("Hello, World!");

async Task BeforeRun(IPage page)
{
    await new ScoutCommand(CommandType.Click , new ScoutCommands(page)  , new ClickQuery {Selector = ".modal-close", WaitForSelector= ".homepage-popup" }).ExecuteAsync();
}

try
{

    var page = await ScoutBuilderDirector
        .NewPage
            .Header(false)
            .With(1920)
            .Height(1080)
            .Url("https://www.aliexpress.us/")
            .Browser("970485")
            .PrepareAsync();   
    var response = await new ScoutCommand(CommandType.JsQuery, new ScoutCommands(page), new JsQuery {Query = "Array.from(document.getElementsByClassName('categories-list-box')[0].getElementsByTagName(\"a\")).map(item => {return item.getAttribute(\"href\")})" })
       .ExecuteAsync();
    var result = JsonConvert.DeserializeObject<List<string>>(response.ResultObject.ToString());

}
catch (Exception ex)
{

	throw;
}