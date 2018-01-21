# Google Trends Snapshot
An automated tool for taking snapshots of the Google Trends line graph.
Use the terminal program to enter terms and effortlessly take snapshots.

NOTE: Currently assumes that you are running on Windows 10 and have Chrome installed.

###Console View:
![alt text](https://github.com/NathanTaylorHunt/GoogleTrendsSnapshot/raw/master/screenshot-console.png "Console Screenshot")

###Resulting Snapshot:
![alt text](https://github.com/NathanTaylorHunt/GoogleTrendsSnapshot/raw/master/screenshot-graph.png "Console Screenshot")

## How to Use
Enter search terms by typing them into the console.  Terms are added to the list.
Type 'snap' to take a snap shot with the previously entered terms.  Typing this will launch Chrome and take a snapshot, saving it to the snapshots folder.
Type 'delete' plus a number to delete a term from the list.
Type 'clear' to clear all terms from the list.
Type 'exit' to quit the program.

## Running the Standalone Exe
For convenience, I built a standalone exe that can be run on Windows 10 without installing any dependencies.
To run it, unzip PREBUILT-WIN10.zip, and run GoogleTrendsSnapshot.exe.
By default, the snapshot files will be saved to the 'snapshots' folder in the same directory as the exe.

## Installation
To run, you must have [dotnet core 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.0-download.md) installed.

## Building and Running

To build:
```
dotnet build
```

To run:
```
dotnet run
```