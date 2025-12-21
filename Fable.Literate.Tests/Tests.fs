module Fable.Literate.Tests.AppTests

#if FABLE_COMPILER
open Fable.Pyxpecto
#else
open Expecto
#endif
open Fable.Literate.App

/// Helper to parse a string into lines
let lines (s: string) = s.Split('\n') |> Array.toSeq

let parserTests =
    testList "Parser" [
        testCase "parses simple markdown block" <| fun _ ->
            let input = lines """(**
Hello world
*)"""
            let result = Parser.parse input
            Expect.equal result [Markdown "Hello world"] "Should parse markdown block"

        testCase "parses single-line markdown" <| fun _ ->
            let input = lines "(** Hello *)"
            let result = Parser.parse input
            Expect.equal result [Markdown "Hello"] "Should parse single-line markdown"

        testCase "parses F# code block" <| fun _ ->
            let input = lines "let x = 42"
            let result = Parser.parse input
            Expect.equal result [FSharpCode ["let x = 42"]] "Should parse code block"

        testCase "parses hide directive" <| fun _ ->
            let input = lines """(*** hide ***)
let secret = 42
(**
Visible
*)"""
            let result = Parser.parse input
            Expect.equal result [
                Hidden ["let secret = 42"]
                Markdown "Visible"
            ] "Should parse hidden block followed by markdown"

        testCase "parses include-python directive" <| fun _ ->
            let input = lines "(*** include-python: foo, bar ***)"
            let result = Parser.parse input
            Expect.equal result [IncludePython ["foo"; "bar"]] "Should parse include-python directive"

        testCase "parses module with body" <| fun _ ->
            let input = lines """module Foo =
    let x = 1"""
            let result = Parser.parse input
            Expect.equal result [FSharpCode ["module Foo ="; "    let x = 1"]] "Should parse module with body"
    ]

let transformTests =
    testList "Transform" [
        testCase "filterHidden removes Hidden blocks" <| fun _ ->
            let doc = [
                Markdown "Before"
                Hidden ["secret"]
                Markdown "After"
            ]
            let result = Transform.filterHidden doc
            Expect.equal result [Markdown "Before"; Markdown "After"] "Should remove hidden blocks"

        testCase "filterBoilerplate removes standalone module declaration" <| fun _ ->
            let doc = [FSharpCode ["module Foo"]]
            let result = Transform.filterBoilerplate doc
            Expect.equal result [] "Should filter standalone module"

        testCase "filterBoilerplate keeps module with body" <| fun _ ->
            let doc = [FSharpCode ["module Foo ="; "    let x = 1"]]
            let result = Transform.filterBoilerplate doc
            Expect.equal result [FSharpCode ["module Foo ="; "    let x = 1"]] "Should keep module with body"

        testCase "filterBoilerplate removes namespace declaration" <| fun _ ->
            let doc = [FSharpCode ["namespace MyLib"]]
            let result = Transform.filterBoilerplate doc
            Expect.equal result [] "Should filter namespace"

        testCase "filterBoilerplate removes empty code blocks" <| fun _ ->
            let doc = [FSharpCode [""]; FSharpCode ["   "]]
            let result = Transform.filterBoilerplate doc
            Expect.equal result [] "Should filter empty code blocks"

        testCase "filterBoilerplate keeps regular code" <| fun _ ->
            let doc = [FSharpCode ["let x = 42"]]
            let result = Transform.filterBoilerplate doc
            Expect.equal result [FSharpCode ["let x = 42"]] "Should keep regular code"
    ]

let utilsTests =
    testList "Utils" [
        testCase "trimCode preserves indentation" <| fun _ ->
            let input = "    let x = 1\n        let y = 2"
            let result = Utils.trimCode input
            Expect.equal result "    let x = 1\n        let y = 2" "Should preserve indentation"

        testCase "trimCode removes leading empty lines" <| fun _ ->
            let input = "\n\n    let x = 1"
            let result = Utils.trimCode input
            Expect.equal result "    let x = 1" "Should remove leading empty lines"

        testCase "trimCode removes trailing whitespace" <| fun _ ->
            let input = "let x = 1\n   "
            let result = Utils.trimCode input
            Expect.equal result "let x = 1" "Should remove trailing whitespace"
    ]

let markdownPrinterTests =
    testList "MarkdownPrinter" [
        testCase "printMarkdown renders F# code block" <| fun _ ->
            let doc = [FSharpCode ["let x = 42"]]
            let result = MarkdownPrinter.printMarkdown doc
            Expect.stringContains result "```fsharp" "Should have fsharp fence"
            Expect.stringContains result "let x = 42" "Should contain code"
    ]

let pipelineTests =
    testList "Pipeline" [
        testCase "full pipeline processes simple document" <| fun _ ->
            let input = lines """(**
# Hello

Some text
*)

let greeting = "world"

(*** hide ***)
let secret = 42

(**
More text
*)"""
            let result = Pipeline.standard None input
            Expect.stringContains result "# Hello" "Should contain heading"
            Expect.stringContains result "Some text" "Should contain text"
            Expect.stringContains result "let greeting" "Should contain visible code"
            Expect.isFalse (result.Contains "secret") "Should not contain hidden code"
            Expect.stringContains result "More text" "Should contain text after hidden section"

        testCase "pipeline preserves indentation in code blocks" <| fun _ ->
            let input = lines """(**
Text
*)

module Foo =
    let x = 1
    let y = 2"""
            let result = Pipeline.standard None input
            Expect.stringContains result "    let x = 1" "Should preserve 4-space indent"
            Expect.stringContains result "    let y = 2" "Should preserve 4-space indent"
    ]
