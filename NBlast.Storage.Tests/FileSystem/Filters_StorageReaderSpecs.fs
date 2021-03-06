﻿namespace NBlast.Storage.Tests.FileSystem

open System
open System.IO
open Lucene.Net.Store
open NBlast.Storage
open NBlast.Storage.Core
open NBlast.Storage.Core.Extensions
open NBlast.Storage.Core.Index
open NBlast.Storage.Core.Env
open NBlast.Storage.FileSystem
open NUnit.Framework
open FluentAssertions
//open NBlast.Storage.Tests.FileSystem

type Filters_StorageReaderSpecs() = 

    [<Test>]
    member me.``After date filter should find nothing when it's out of scope``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.After(date) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(0, "Should return 0 hits") |> ignore

    [<Test>]
    member me.``After date filter should find only expected values``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.After(date.AddDays(-4.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(3, "Should return 3 hits") |> ignore
        hits.Hits.[0].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-1.0).DayOfYear, "Days must be equal") |> ignore
        hits.Hits.[1].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-2.0).DayOfYear, "Days must be equal") |> ignore
        hits.Hits.[2].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-3.0).DayOfYear, "Days must be equal") |> ignore

    [<Test>]
    member me.``Before date filter should find nothing when it's out of scope``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.Before(date.AddDays(-10.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(0, "Should return 0 hits") |> ignore

    [<Test>]
    member me.``Before date filter should find only expected values``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.Before(date.AddDays(-4.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(2, "Should return 2 hits") |> ignore
        hits.Hits.[0].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-5.0).DayOfYear, "Days must be equal") |> ignore
        hits.Hits.[1].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-6.0).DayOfYear, "Days must be equal") |> ignore

    [<Test>]
    member me.``Between date filter should find nothing when it's out of scope``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.Between(date.AddDays(-20.0), date.AddDays(-10.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(0, "Should return 0 hits") |> ignore

    [<Test>]
    member me.``Between date filter should find only expected values``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression() with 
                Filter = FilterQuery.Between(date.AddDays(-5.0), date.AddDays(-2.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(2, "Should return 2 hits") |> ignore
        hits.Hits.[0].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-3.0).DayOfYear, "Days must be equal") |> ignore
        hits.Hits.[1].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-4.0).DayOfYear, "Days must be equal") |> ignore

    [<Test>]
    member me.``Combining an ordinary search query and between date filter should find only expected values``() =
        // Given
        let path = me.GenerateTempPath()
        let writer = new StorageWriter(me.MakeDirectoryProvider(path)) :> IStorageWriter
        let sut = me.MakeStorageReader(path)
        let date = DateTime.Now
        let query = {
            SearchQuery.GetOnlyExpression("4 BCD") with 
                Filter = FilterQuery.Between(date.AddDays(-6.0), date.AddDays(-1.0)) |> Some 
        }
        
        // When
        me.``gimme 6 fake documents``(date) |> writer.InsertMany
        let hits = sut.SearchByField query
        
        // Then
        hits.Total.Should().Be(1, "Should return 1 hits") |> ignore
        hits.Hits.[0].CreatedAt.DayOfYear.Should()
            .Be(date.AddDays(-4.0).DayOfYear, "Days must be equal") |> ignore


    member private this.``gimme 6 fake documents`` (?date: DateTime) =
        let date = DateTime.Now |> defaultArg date
        [ new LogDocument("sender1", "1 C C", "log", "debug", None, date.AddDays(-1.0) |> Some);
          new LogDocument("sender1", "2 B", "log", "debug", None, date.AddDays(-2.0) |> Some);
          new LogDocument("sender1", "3 C", "log", "debug", None, date.AddDays(-3.0) |> Some);
          new LogDocument("sender2", "4 BCD", "log", "debug", None, date.AddDays(-4.0) |> Some);
          new LogDocument("sender2", "5 C", "log", "debug", None, date.AddDays(-5.0) |> Some);
          new LogDocument("sender2", "6 B", "log", "debug", None, date.AddDays(-6.0) |> Some); ] 
            |> List.map (fun x -> x :> IStorageDocument)