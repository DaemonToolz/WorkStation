package main

import (
	"encoding/json"
	"fmt"
	"log"
	"sync"
	"testing"
	"time"
)

func TestReadDir(t *testing.T) {
	tests := []struct {
		DirPath string
	}{
		{"..\\WorkstationBrowser\\UserContent\\FileTracker\\Workstation\\Comments\\"},
	}
	for _, tt := range tests {
		start := time.Now()

		jsonMarshalled, _ := json.Marshal(ReadDir(tt.DirPath, ""))
		elapsed := time.Since(start)
		fmt.Println(string(jsonMarshalled))
		log.Printf("Function took %s", elapsed)
	}
}

func Test_grReadDir(t *testing.T) {
	tests := []struct {
		DirPath string
	}{
		{"..\\WorkstationBrowser\\UserContent\\FileTracker\\Workstation\\Comments\\"},
	}
	// TODO: Add test cases.
	qChannel := make(chan Query)
	var wg sync.WaitGroup

	for _, tt := range tests {
		start := time.Now()
		wg.Add(1)
		go grReadDir(tt.DirPath, "", qChannel, &wg)

		jsonMarshalled, _ := json.Marshal(<-qChannel)
		elapsed := time.Since(start)
		fmt.Println(string(jsonMarshalled))
		log.Printf("Function took %s", elapsed)
	}
}

func Test_grReadDirOld(t *testing.T) {
	tests := []struct {
		DirPath string
	}{
		{"..\\WorkstationBrowser\\UserContent\\FileTracker\\Workstation\\Comments\\"},
	}
	// TODO: Add test cases.
	qChannel := make(chan Query)
	var wg sync.WaitGroup

	for _, tt := range tests {
		start := time.Now()
		wg.Add(1)
		go grReadDirOld(tt.DirPath, "", qChannel, &wg)

		jsonMarshalled, _ := json.Marshal(<-qChannel)
		elapsed := time.Since(start)
		fmt.Println(string(jsonMarshalled))
		log.Printf("Function took %s", elapsed)
	}
}
