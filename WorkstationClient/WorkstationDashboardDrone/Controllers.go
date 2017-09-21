package main

import (
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"os"
)

func ReadDir(DirPath string) Query {
	files, err := ioutil.ReadDir(DirPath)
	if err != nil {
		fmt.Println("Error opening file:", err)
		return Query{}
	}

	query := Query{}

	query.Files = make([]FileDescription, 0)

	for _, f := range files {
		if f.IsDir() == false {

			name := f.Name()
			cmts := ReadFile(DirPath + "\\" + f.Name())

			descriptor := FileDescription{}
			descriptor.Name = name
			descriptor.Comments = cmts

			query.Files = append(query.Files, descriptor)
		}
	}

	return query
}

//
func ReadFile(Filepath string) Comments {
	filename := Filepath
	xmlFile, err := os.Open(filename)

	if err != nil {
		fmt.Println("Error opening file:", err)
		return Comments{}
	}

	defer xmlFile.Close()
	b, _ := ioutil.ReadAll(xmlFile)
	var root Root
	xml.Unmarshal(b, &root.Root)
	return root.Root
	//jsonMarshalled, _ := json.Marshal(root)

	//return string(jsonMarshalled)
}
