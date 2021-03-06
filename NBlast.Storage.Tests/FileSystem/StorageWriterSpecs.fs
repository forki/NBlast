﻿namespace NBlast.Storage.Tests.FileSystem

open System
open System.Runtime
open NUnit.Framework
open FluentAssertions
open NBlast.Storage
open NBlast.Storage.FileSystem
open NBlast.Storage.Core
open NBlast.Storage.Core.Index
open NBlast.Storage.Core.Env
open NBlast.Storage.Core.Exceptions
open System.IO
open Lucene.Net.Documents
open Lucene.Net.Store
open Lucene.Net.Util
open Lucene.Net.Index
open Lucene.Net.Analysis.Standard

type FakeDocument() = 
    interface IStorageDocument with 
        member this.ToLuceneDocument() = 
            let document = new Document()
            document.Add(new Field("Id", System.Guid.NewGuid().ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED))
            document

type StorageWriterSpecs() = 

    [<Test>]
    member this.``Writer directory must be created as expected``() =
        // Given
        let path = this.GenerateTempPath()
        let sut  = (this.MakeStorageWriter false path) 

        // When
        sut.InsertOne(FakeDocument())

        // Then
        (Directory.Exists(path)).Should().Be(true, sprintf "Storage has to create its directory in %s" path) |> ignore
    (*
    [<Test>]
    member this.``Writer directory creation must fail trying unlock locked directory``() =
        // Given
        let path = this.GenerateTempPath()
        let sut  = (this.MakeSut true path) 
        let directory = FSDirectory.Open(new DirectoryInfo(path))

        // When
        Directory.CreateDirectory(path) |> ignore
        directory.MakeLock(IndexWriter.WRITE_LOCK_NAME).Obtain(0L) |> ignore
        directory.Dispose()

        Assert.Throws<StorageUnlockFailedException>(fun () -> sut.InsertOne(FakeDocument())) 
    *)
    [<Test>]
    member this.``Writer directory creation must fail when it's already locked``() =
        // Given
        let path = this.GenerateTempPath()
        let sut  = (this.MakeStorageWriter false path) 
        let directory = FSDirectory.Open(new DirectoryInfo(path))

        // When
        Directory.CreateDirectory(path) |> ignore
        directory.MakeLock(IndexWriter.WRITE_LOCK_NAME).Obtain(0L) |> ignore
        directory.Dispose()

        Assert.Throws<StorageLockedException>(fun () -> sut.InsertOne(FakeDocument())) |> ignore
