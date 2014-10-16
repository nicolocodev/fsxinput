open Xinput
open System 

[<EntryPoint>]
let main argv =
    let controller = Xinput.Controllers.Controller(0)
    let isConnected = controller.IsConnected
    if isConnected then printfn "%s" "Connected control"
    else printf "%s" "Disconnected control"
    controller.OnButtonPress.Add(fun args -> printfn "%A" args.ControllerButton)
    //You can use observable to query the events stream
    //let observable = controller.OnButtonPress
    //observable |> Observable.filter(fun args -> args.ControllerButton = ControllerButtons.A) |> Observable.subscribe(fun _ -> controller.Vibrate 30000us 3000us) |> ignore
    controller.Start    
    Console.ReadLine() |> ignore
    0 // return an integer exit code
