﻿namespace NBlast.Api.Models

open Newtonsoft.Json
open System
open System.Net
open System.Web.Http.ModelBinding
open System.ComponentModel.DataAnnotations
open System.ComponentModel
open System.Collections.Specialized

type LogModel () =
    [<Required>]
    [<field: JsonProperty("sender")>]
    member val Sender: string = null with get, set
    
    [<Required>]
    [<field: JsonProperty("message")>]
    member val Message: string = null with get, set
    
    [<Required>]
    [<field: JsonProperty("logger")>]
    member val Logger: string = null with get, set
    
    [<Required>]
    [<field: JsonProperty("level")>]
    member val Level: string = null with get, set

    [<field: JsonProperty("error")>]
    member val Error: string = null with get, set 

    [<field: JsonProperty("createdAt")>]
    member val CreatedAt: Nullable<DateTime> = Unchecked.defaultof<_> with get, set 

    [<JsonIgnore>]
    member me.ErrorOp with get() = if(String.IsNullOrEmpty(me.Error)) then None else Some me.Error

    [<JsonIgnore>]
    member me.CreatedAtOp with get() = if (me.CreatedAt.HasValue) then Some me.CreatedAt.Value else None

    static member BuildFromParams(collection: NameValueCollection) = 
        let createdAt = 
            match DateTime.TryParse(collection.["createdAt"]) with
                | (true, dateTime) -> new Nullable<DateTime>(dateTime)
                | _ -> Unchecked.defaultof<_>

        new LogModel(
            Sender  = collection.["sender"],
            Logger  = collection.["logger"],
            Level   = collection.["level"],
            Error   = collection.["error"],
            Message = collection.["message"]
        )

    override me.ToString() = 
        sprintf "{sender=%s, logger=%s, level=%s, message=%s, createdAt=%s}" 
                    me.Sender 
                    me.Logger
                    me.Level
                    me.Message
                    (if me.CreatedAt.HasValue then me.CreatedAt.Value.ToString() else "<NULL>")