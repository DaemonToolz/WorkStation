package main

import (
	"encoding/json"
	"fmt"
	"testing"
)

type Query struct {
	Files []FileDescription `json:"files"`
}

type FileDescription struct {
	Name     string   `json:"Name"`
	Comments Comments `json:"Root"`
}

func TestReadDir(t *testing.T) {
	tests := []struct {
		DirPath string
	}{
		{"..\\WorkstationBrowser\\UserContent\\FileTracker\\Workstation\\Comments\\"},
	}
	for _, tt := range tests {
		res := Query{}
		json.Unmarshal([]byte(ReadDir(tt.DirPath)), &res)
		fmt.Println(res.Files)
	}
}
