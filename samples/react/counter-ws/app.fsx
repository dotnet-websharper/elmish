(**
 - title: Counter
 - tagline: The famous Increment/Decrement ported from Elm
*)


#r "../node_modules/fable-core/Fable.Core.dll"
#r "../node_modules/fable-powerpack/Fable.PowerPack.dll"
#r "../node_modules/fable-react/Fable.React.dll"
#r "../node_modules/fable-elmish/Fable.Elmish.dll"
#r "../node_modules/fable-elmish-react/Fable.Elmish.React.dll"
#r "Fable.Import.Ws.fs"
#r "Fable.Helpers.Ws.fs"

open Fable.Core
open Fable.Import
open Elmish
open Fable.Helpers.Ws

// MODEL

type Msg =
  | Increment
  | Decrement

let init () = 0

// UPDATE

let update (msg:Msg) count =
  match msg with
  | Increment -> count + 1
  | Decrement -> count - 1

// rendering views with React
module R = Fable.Helpers.React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props

let view count dispatch =
  let onClick msg =
    OnClick <| fun _ -> msg |> dispatch

  R.div [] [
      R.div [] [ unbox "Testing!" ] 
      R.button [ onClick Decrement ] [ unbox "-" ]
      R.div [] [ unbox (string count) ]
      R.button [ onClick Increment ] [ unbox "+" ] 
  ]

open Elmish.React

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.run