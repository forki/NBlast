﻿namespace NBlast.Api.Tests.Async

open NBlast.Api.Async
open NBlast.Api.Models
open NBlast.Storage.Core.Extensions
open System
open System.Runtime
open NUnit.Framework
open FluentAssertions
open Ploeh.AutoFixture
open Ploeh.AutoFixture.Kernel
open System.Threading.Tasks
open System.Linq

type IndexingQueueKeeperSpecs() =

    [<Test>]
    member me.``Keeper must enqueue models in the ordinary context``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``()
        
        // When
        models |> Seq.iter (fun x -> sut.Enqueue(x))
         
        // Then
        sut.Count().Should().Be(10, "Queue count must be 10") |> ignore

    [<Test>]
    member me.``Keeper must enqueue models in the parallel context``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``()
        
        // When
        models |> Seq.iter (fun x -> sut.Enqueue(x))

        // Then
        sut.Count().Should().Be(10, "Queue count must be 10") |> ignore

    [<Test>]
    member me.``Keeper must peek top indicated model contained in the queue``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models`` 5
        
        // When
        models |> List.iter (fun x -> sut.Enqueue(x)) 
        let actuals = sut.PeekTop(10)

        // Then
        sut.Count().Should().Be(5, "Queue count must be 5") |> ignore
        (actuals |> Seq.length).Should().Be(5, "Actuals must count 5") |> ignore
        

    [<Test>]
    member me.``Keeper must peek model and leave it in queue``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``()
        
        // When
        models |> List.iter (fun x -> sut.Enqueue(x)) 
        let actual = sut.Peek()

        // Then
        sut.Count().Should().Be(10, "Queue count must be 10") |> ignore
        actual.IsSome.Should().BeTrue("Actual cannot be None") |> ignore
        actual.Value.Should().Be(models.Head, "List head and peeken element is the same") |> ignore

    [<Test>]
    member me.``Keeper must consume models in the ordinary context``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``(2)
        
        // When
        models |> Seq.iter (fun x -> sut.Enqueue(x))
        let actuals = [sut.Consume(); sut.Consume(); sut.Consume()]

        // Then
        sut.Count().Should().Be(0, "Queue count must remain 0 after consumption") |> ignore
        (actuals |> List.head |> Option.isSome).Should().BeTrue("First element of actuals cannot be None") |> ignore
        ((actuals |> List.toArray).[1] |> Option.isSome).Should().BeTrue("Second element of actuals cannot be None") |> ignore
        ((actuals |> List.toArray).[2] |> Option.isNone).Should().BeTrue("Third element of actuals must be None") |> ignore


    [<Test>]
    member me.``Keeper must consume models in parallel context as well and without locking``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``(100)
        
        // When
        let actuals = models.AsParallel().Select(fun x -> sut.Enqueue(x) ; sut.Consume()) 
                    |> Seq.toList 
                    |> List.filter (fun x -> x.IsSome && x.Value |> ``is ∅`` |> not)
        
        // Then
        (actuals |> List.length).Should().Be(100, "Actual models count must be equal 100") |> ignore

    [<Test>]
    member me.``Keeper must consume many models in the ordinary context``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``(5)
        
        // When
        models |> Seq.iter (fun x -> sut.Enqueue(x) |> ignore)
        let actuals = sut.ConsumeMany(Some 4) |> Seq.toList

        // Then
        (actuals |> List.length).Should().Be(4, "Consume many must extract exactly 4 models") |> ignore
        sut.Count().Should().Be(1, "Queue count must be equal 1") |> ignore
        actuals |> List.iter (fun x -> (x).Should().NotBeNull("") |> ignore)

    [<Test>]
    member me.``Keeper must try to consume many models and return nothing with an empty keeper``() =
        // Given
        let sut = me.MakeSut()
        let models = me.``Gimme N log models``(5)
        
        // When
        let actuals = sut.ConsumeMany(Some 5) |> Seq.toList

        // Then
        (actuals |> List.length).Should().Be(0, "Consume many must extract nothing") |> ignore
        sut.Count().Should().Be(0, "Queue must remain empty") |> ignore

    
    member private me.``Gimme N log models`` ?x =
        (new Fixture() :> IFixture).CreateMany<LogModel>(defaultArg x 10) |> Seq.toList

    member private me.MakeSut(): IIndexingQueueKeeper = 
        new IndexingQueueKeeper() :> IIndexingQueueKeeper