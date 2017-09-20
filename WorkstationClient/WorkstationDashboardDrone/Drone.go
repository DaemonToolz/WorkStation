package main

import (
	"bytes"
	"encoding/json"
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"

	// Git repos here
	"github.com/gorilla/mux"
)

// Models

type Root struct {
	Root Comments `xml:"root", json:"Root"`
}

type Comments struct {
	Comments []Comment `xml:"comments", json:"Comments"`
}

type Comment struct {
	Content Content `xml:"comment", json:"Comment"`
}

type Content struct {
	Author  string `xml:"author", json:"Author"`
	Date    string `xml:"date", json:"Date"`
	Content string `xml:"content", json:"Content""`
}

func (e Comment) String() string {
	return fmt.Sprintf("{date:%s;author:%s;content:%s}", e.Content.Date, e.Content.Author, e.Content.Content)
}

func (e Comments) String() string {
	//return fmt.Sprintf("{date:%s;author:%s;content:%s}", e.Content.Date, e.Content.Author, e.Content.Content)
	var buffer bytes.Buffer
	buffer.WriteString("comments:[")

	for index, comment := range e.Comments {
		buffer.WriteString(comment.String())
		if index < (len(e.Comments) - 1) {
			buffer.WriteString(",")
		}
	}

	buffer.WriteString("]")

	return buffer.String()
}

func (e Root) String() string {
	//return fmt.Sprintf("{date:%s;author:%s;content:%s}", e.Content.Date, e.Content.Author, e.Content.Content)
	var buffer bytes.Buffer
	buffer.WriteString("root:[")
	buffer.WriteString(e.Root.String())
	buffer.WriteString("]")
	return buffer.String()
}

//

func main() {
	router := mux.NewRouter().StrictSlash(true)
	router.HandleFunc("/Comments/{projectName}", AllComments)

	log.Fatal(http.ListenAndServe(":10450", router))

}

//
func AllComments(w http.ResponseWriter, r *http.Request) {
	//vars := mux.Vars(r)
	//projectName := vars["projectName"]

	// Find workaround
	//concretePath := "..\\WorkstationBrowser\\UserContent\\FileTracker\\" + projectName + "\\Comments\\"
	//result := ReadDir(concretePath)
}

// Functions

func ReadDir(DirPath string) string {

	files, err := ioutil.ReadDir(DirPath)
	if err != nil {
		fmt.Println("Error opening file:", err)
		return "{}"
	}

	var buffer bytes.Buffer
	buffer.WriteString("{\"files\":[")
	for x, f := range files {
		if f.IsDir() == false {
			buffer.WriteString("{\"Name\":\"" + f.Name() + "\",")
			result := ReadFile(DirPath + "\\" + f.Name())
			buffer.WriteString(result[1 : len(result)-1])
			buffer.WriteString("}")
			if x < (len(files) - 1) {
				buffer.WriteString(",")
			}
		}
	}
	buffer.WriteString("]}")
	return buffer.String()
}

//
func ReadFile(Filepath string) string {
	filename := Filepath
	xmlFile, err := os.Open(filename)

	if err != nil {
		fmt.Println("Error opening file:", err)
		return "{}"
	}

	defer xmlFile.Close()
	b, _ := ioutil.ReadAll(xmlFile)
	var q Root
	xml.Unmarshal(b, &q.Root)

	jsonMarshalled, _ := json.Marshal(q)

	return string(jsonMarshalled)
	/*for _, comment := range q.Root.Comments {
		fmt.Printf("\t%s\n", comment)
	}*/
}
