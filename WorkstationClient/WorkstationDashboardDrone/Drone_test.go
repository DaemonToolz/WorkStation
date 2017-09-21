package main

import (
	"encoding/json"
	"fmt"
	"testing"
)

func TestReadDir(t *testing.T) {
	tests := []struct {
		DirPath string
	}{
		{"..\\WorkstationBrowser\\UserContent\\FileTracker\\Workstation\\Comments\\"},
	}
	for _, tt := range tests {

		jsonMarshalled, _ := json.Marshal(ReadDir(tt.DirPath))

		fmt.Println(string(jsonMarshalled))
	}
}
