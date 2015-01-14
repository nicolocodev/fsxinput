open Xinput
open System 

[<EntryPoint>]
let main argv =

    let controller = Xinput.Controllers.Controller(1)
    let isConnected = controller.IsConnected

    let startUsingController() =

        printfn "%s" "Connected control"

        controller.OnDisconnect.Add(fun _ -> 
            Console.WriteLine("Disconnected control - Event")
            Console.ReadLine() |> ignore)

        //You can use observable to query the events stream
        let observable = controller.OnButtonPress    

        let buttonPress = 
            observable 
            |> Observable.subscribe(fun b -> printfn "%A" b.ControllerButton)                         
        
        //Press Guide button to turn off
        let guideButton = 
            observable 
            |> Observable.filter(fun args -> args.ControllerButton = ControllerButtons.Guide) 
            |> Observable.subscribe(fun _ -> controller.Turnoff)             

        controller.Start 100.0

    if isConnected then 
        startUsingController()
    else 
        printf "%s" "Disconnected control"
        Console.ReadLine() |> ignore
    
    Console.ReadLine() |> ignore
    0 // return an integer exit code