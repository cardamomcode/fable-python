module Fable.Literate.Tests.Program

#if FABLE_COMPILER
open Fable.Pyxpecto
#else
open Expecto
#endif

open Fable.Literate.Tests.AppTests

let allTests =
    testList "All" [
        parserTests
        transformTests
        utilsTests
        markdownPrinterTests
        pipelineTests
    ]

[<EntryPoint>]
let main args =
#if FABLE_COMPILER
    Pyxpecto.runTests [||] allTests
#else
    runTestsWithCLIArgs [] args allTests
#endif
