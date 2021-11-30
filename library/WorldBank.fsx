(**
// can't yet format YamlFrontmatter (["category: Type Providers"; "categoryindex: 1"; "index: 5"], Some { StartLine = 2 StartColumn = 0 EndLine = 5 EndColumn = 8 }) to pynb markdown

*)
#r "nuget: FSharp.Data,4.2.6"
(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/fsprojects/FSharp.Data/gh-pages?filepath=library/WorldBank.ipynb)&emsp;
[![Script](../img/badge-script.svg)](https://fsprojects.github.io/FSharp.Data//library/WorldBank.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](https://fsprojects.github.io/FSharp.Data//library/WorldBank.ipynb)

# WorldBank Provider

The [World Bank](http://www.worldbank.org) is an international organization that provides
financial and technical assistance to developing countries around the world. As one of the
activities, the World Bank also collects development indicators and other data about
countries in the world. The [data catalog](http://data.worldbank.org/) contains over
8,000 indicators that can be programmatically accessed.

The WorldBank Type Provider makes the WorldBank data easily accessible to F# programs and
scripts in a type-safe manner. This article provides an introduction.

## Introducing the provider

*)
<div class="container-fluid" style="margin:15px 0px 15px 0px;">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10" id="anim-holder">
            <a id="lnk" href="../images/wb.gif"><img id="anim" src="../images/wb.gif" /></a>
        </div>
        <div class="span1"></div>
    </div>
</div>
(**
The following example initializes a connection to the WorldBank using the `GetDataContext` method and then
retrieves the percentage of population who attend universities in the UK:

*)
open FSharp.Data

let data = WorldBankData.GetDataContext()

data
  .Countries.``United Kingdom``
  .Indicators.``Gross capital formation (% of GDP)``
|> Seq.maxBy fst(* output: 
val data : WorldBankData.ServiceTypes.WorldBankDataService
val it : int * float = (2020, 16.9637966)*)
(**
When generating the data context, the WorldBank Type Provider retrieves the list of all
countries known to the WorldBank and the list of all supported indicators. Both of these
dimensions are provided as properties, so you can use autocomplete to easily discover
various data sources. Most of the indicators use longer names, so we need to wrap the name
in `\```.

The result of the `Gross capital formation (% of GDP)` property is a sequence with
values for different years. Using `Seq.maxBy fst` we get the most recent available value.

## Using World Bank data asynchronously

If you need to download large amounts of data or run the operation without
blocking the caller, then you probably want to use F# asynchronous workflows to perform
the operation. The FSharp.Data package also provides the `WorldBankDataProvider` type which takes
a number of static parameters. If the `Asynchronous` parameter is set to `true` then the
type provider generates all operations as asynchronous:

*)
type WorldBank = WorldBankDataProvider<"World Development Indicators", Asynchronous=true>
WorldBank.GetDataContext()
(**
The above snippet specified "World Development Indicators" as the name of the data
source (a collection of commonly available indicators) and it set the optional argument
`Asynchronous` to `true`. As a result, properties such as
`Gross capital formation (% of GDP)` will now have a type `Async<(int * int)[]>` meaning
that they represent an asynchronous computation that can be started and will eventually
produce the data.

### Downloading data in parallel

To demonstrate the asynchronous version of the type provider, let's write code that
downloads the university enrollment data about a number of countries in parallel.
We first create a data context and then define an array with some countries we want to
process:

*)
let wb = WorldBank.GetDataContext()

// Create a list of countries to process
let countries =
 [| wb.Countries.``Arab World``
    wb.Countries.``European Union``
    wb.Countries.Australia
    wb.Countries.Brazil
    wb.Countries.Canada
    wb.Countries.Chile
    wb.Countries.``Czech Republic``
    wb.Countries.Denmark
    wb.Countries.France
    wb.Countries.Greece
    wb.Countries.``Low income``
    wb.Countries.``High income``
    wb.Countries.``United Kingdom``
    wb.Countries.``United States`` |]
(**
To download the information in parallel, we can create a list of asynchronous
computations, compose them using `Async.Parallel` and then run the (single) obtained
computation to perform all the downloads:

*)
[ for c in countries ->
    c.Indicators.``Gross capital formation (% of GDP)`` ]
|> Async.Parallel
|> Async.RunSynchronously(* output: 
val it : Runtime.WorldBank.Indicator [] =
  [|Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP);
    Gross capital formation (% of GDP); Gross capital formation (% of GDP)|]*)
(**
## Related articles

// can't yet format Span ([Literal ("API Reference: ", Some { StartLine = 4 StartColumn = 3 EndLine = 5 EndColumn = 18 }); DirectLink ([Literal ("WorldBankDataProvider", Some { StartLine = 4 StartColumn = 19 EndLine = 5 EndColumn = -3 })], "https://fsprojects.github.io/FSharp.Data/reference/fsharp-data-worldbankdataprovider.html", None, Some { StartLine = 4 StartColumn = 19 EndLine = 5 EndColumn = -3 }); Literal (" type provider", Some { StartLine = 4 StartColumn = 18 EndLine = 5 EndColumn = 32 })], Some { StartLine = 4 StartColumn = 0 EndLine = 4 EndColumn = 74 }) to pynb markdown
*)

