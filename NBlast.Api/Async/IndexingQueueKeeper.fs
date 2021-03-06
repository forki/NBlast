namespace NBlast.Api.Async

open NBlast.Api.Models
open NBlast.Storage
open NBlast.Storage.Core.Index
open NBlast.Storage.Core.Extensions
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading.Tasks
open System

type IQueueKeeper<'a> = interface
    abstract member Enqueue : 'a -> unit
    abstract member Consume : unit -> 'a option
    abstract member ConsumeMany : int option -> seq<'a>
    abstract member Peek : unit -> 'a option
    abstract member Count : unit -> int 
    abstract member PeekTop : int -> seq<'a>
end

type IIndexingQueueKeeper = inherit IQueueKeeper<LogModel>

type IndexingQueueKeeper() =
    static let logger = NLog.LogManager.GetCurrentClassLogger()
    let queue = new ConcurrentQueue<LogModel>()

    interface IIndexingQueueKeeper with
        member me.Count() = queue |> Seq.length 
        member me.PeekTop top = 
            queue |> Seq.truncate top
        member me.Peek() =  
            match queue.TryPeek() with 
            | (false, _) -> None
            | (true, model) -> Some model

        member me.Enqueue model = 
                model |> sprintf "Enqueuing model %O" |> logger.Debug
                queue.Enqueue(model)

        member me.Consume() = 
            match queue.TryDequeue() with
            | (false, _) -> None
            | (true, model) ->
                //model |> sprintf "Dequeuing model %A" |> logger.Debug 
                Some model

        member me.ConsumeMany ?amount =
            let consume = (me :> IIndexingQueueKeeper).Consume
            [ 0 .. (amount |? 10) - 1 ] |> Seq.map (fun x -> consume()) 
                |> Seq.filter Option.isSome 
                |> Seq.map Option.get 
            