
open System
open System.IO
open System.Configuration

let StateFile argv = Path.Combine(argv, "state.json")
let MoveFile argv = Path.Combine(argv, "move.txt")

let LoadState path = 
    File.ReadLines path
     |> Seq.toArray
     |> String.concat ""

let WriteMove path =
    let rnd = System.Random()
    let move = rnd.Next(0, 6)
    let moveFile = Path.Combine(path, "move.txt")
    File.WriteAllText(path, move.ToString())

[<EntryPoint>]
let main argv = 

    printf "Player Key %s \n" argv.[0]
    printf "Player Path %s \n" argv.[1]

    let stopwatch = Diagnostics.Stopwatch.StartNew();

    let stateFile = StateFile(argv.[1])
    let moveFile = MoveFile(argv.[1])

    let state = LoadState(stateFile)
    WriteMove(moveFile)

    stopwatch.Stop()
    printfn "[BOT]\tBot finished in %d ms." stopwatch.ElapsedMilliseconds
    0 // return an integer exit code