package main

import (
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
)

// Services
func AllComments(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	projectName := vars["projectName"]

	// Find workaround"
	concretePath :=
		"C:\\Users\\macie\\Source\\Repos\\WorkStation\\WorkstationClient\\WorkstationBrowser\\UserContent\\FileTracker\\" + projectName + "\\Comments\\"
	result := ReadDir(concretePath, "")

	w.Header().Set("Content-Type", "application/json; charset=UTF-8")
	w.WriteHeader(http.StatusOK)

	if err := json.NewEncoder(w).Encode(result); err != nil {
		panic(err)
	}
}
