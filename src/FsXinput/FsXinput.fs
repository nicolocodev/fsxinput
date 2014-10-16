module Xinput
open System.Security
open System.Runtime.InteropServices
open System

[<Flags>]
type ControllerButtons = 
    | Up = 0x00000001
    | Down = 0x00000002
    | Left = 0x00000004
    | Right = 0x00000008
    | Start = 0x00000010
    | Back = 0x00000020
    | ThumbLeft = 0x00000040
    | ThumbRight = 0x00000080
    | ShoulderLeft = 0x0100
    | ShoulderRight = 0x0200
    | A = 0x1000
    | B = 0x2000
    | X = 0x4000
    | Y = 0x8000
    | None = 0

type Vibatrion = 
    struct
        val mutable LeftMotorSpeed : uint16
        val mutable RightMotorSpeed : uint16
    end

type GamePad = 
    struct
        val Buttons: ControllerButtons
        val LeftTrigger : byte       
        val RightTrigger : byte
        val ThumbLeftX : int16
        val ThumbLeftY : int16
        val ThumbRightX : int16
        val ThumbRightY : int16
    end

type State =
    struct
        val mutable PacketNumber:uint32
        val mutable GamePad:GamePad        
    end

[<SuppressUnmanagedCodeSecurity>]
[<DllImport("xinput1_4.dll", EntryPoint="XInputGetState", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)>]
extern int internal GetState(int dwUserIndex, State& pState)

[<SuppressUnmanagedCodeSecurity>]
[<DllImport("xinput1_4.dll", EntryPoint="XInputSetState", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)>]
extern int internal SetState(int dwUserIndex, Vibatrion& pVibration)

type ButtonEventArgs(controllerButton) =
    let button = controllerButton
    with member this.ControllerButton = button        

type Controller(playerIndex:int) =     
    let _playerIndex = playerIndex    
    let mutable state = new State()
    let mutable vibration = new Vibatrion()
    let mutable pollInterval = 100

    let onButtonPress = new Event<ButtonEventArgs>()
    let trigger button = onButtonPress.Trigger (new ButtonEventArgs(button))    
    
    member this.PollInterval 
        with get() = pollInterval
        and set(value) = pollInterval <- value

    member this.Start = 
        let poll =
            let timer = new System.Timers.Timer(float pollInterval)
            timer.AutoReset <- true        
            let observable = timer.Elapsed  
            let task = async {timer.Start()}
            (task,observable)        
        let tmr, stream = poll
        stream |> Observable.subscribe (fun _ -> 
                    this.PollState |> ignore
                    match state.GamePad.Buttons with
                    | ControllerButtons.None -> ()                
                    | _ -> trigger state.GamePad.Buttons) 
                |> ignore 
        Async.RunSynchronously tmr


    [<CLIEvent>]
    member this.OnButtonPress = onButtonPress.Publish      

    member this.PollState =
        GetState(_playerIndex, &state) = 0
    
    member this.IsConnected = this.PollState
    
    member this.LeftMotorSpeed
        with get() = vibration.LeftMotorSpeed
        and set(value) =                    
            vibration.LeftMotorSpeed <- value
            SetState(_playerIndex, &vibration) |> ignore     

    member this.RightMotorSpeed
        with get() = vibration.RightMotorSpeed
        and set(value) = 
            vibration.RightMotorSpeed <- value
            SetState(_playerIndex, &vibration) |> ignore            

    member this.State =
        this.PollState |> ignore
        state

    member this.Vibrate leftMotorSpeed rightMotorSpeed =
        vibration.LeftMotorSpeed <- leftMotorSpeed
        vibration.RightMotorSpeed <- rightMotorSpeed 
        SetState(_playerIndex, &vibration) |> ignore     
        ()

type ControllerCollection() =
    let controllers = [new Controller(0); new Controller(1); new Controller(2); new Controller(3)]
    member this.Controller with get(index) = controllers.[index]

type Xinput() =
    static let error = 0    
    static let controllerList = new ControllerCollection()    
    static member Controllers = controllerList
    static member Error = error