package main

import (
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"os"
	"sync"
)

func ReadDir(DirPath string, parent string) Query {
	files, err := ioutil.ReadDir(DirPath)
	if err != nil {
		fmt.Println("Error opening file:", err)
		return Query{}
	}

	query := Query{}

	query.Files = make([]FileDescription, 0)

	for _, f := range files {
		if f.IsDir() == false {

			name := parent + "/" + f.Name()
			cmts := ReadFile(DirPath + "\\" + f.Name())

			descriptor := FileDescription{}
			descriptor.Name = name
			descriptor.Comments = cmts

			query.Files = append(query.Files, descriptor)
		} else {
			query.Files = append(query.Files, ReadDir(DirPath+"\\"+f.Name(), parent+"/"+f.Name()).Files...)
		}
	}

	return query
}

//
func ReadFile(Filepath string) Comments {
	xmlFile, err := os.Open(Filepath)

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

// Goroutines related
func grReadFile(Filepath string, name string, cChannel chan FileDescription, wg *sync.WaitGroup) {
	defer wg.Done()
	xmlFile, err := os.Open(Filepath)

	if err != nil {
		fmt.Println("Error opening file:", err)
		cChannel <- FileDescription{}
		return
	}

	defer xmlFile.Close()
	b, _ := ioutil.ReadAll(xmlFile)
	var root Root
	xml.Unmarshal(b, &root.Root)

	cChannel <- FileDescription{Name: name, Comments: root.Root}

}

func grReadDir(DirPath string, parent string, result chan Query, wg *sync.WaitGroup) {
	defer wg.Done() // Done at the end, ofc

	files, err := ioutil.ReadDir(DirPath)
	if err != nil {
		fmt.Println("Error opening file:", err)
		result <- (Query{})

		return // Do not continue
	}

	// Preparing the final query
	query := Query{}
	query.Files = make([]FileDescription, 0)

	// Preparing space for routines
	totalLocks := 0
	totalFiles := 0

	qChannel := make(chan Query, len(files))
	fChannel := make(chan FileDescription, len(files))

	var thisWaitingGroup sync.WaitGroup
	thisWaitingGroup.Add(len(files))

	for _, f := range files {
		if f.IsDir() == false {
			totalFiles++
			go grReadFile(DirPath+"\\"+f.Name(), parent+"/"+f.Name(), fChannel, &thisWaitingGroup)

		} else {

			totalLocks++
			go grReadDir(DirPath+"\\"+f.Name(), parent+"/"+f.Name(), qChannel, &thisWaitingGroup)
		}
	}

	thisWaitingGroup.Wait()
	//defer thisWaitingGroup.Wait()
	close(qChannel)
	close(fChannel)
	if totalLocks > 0 {
		for response := range qChannel {
			query.Files = append(query.Files, response.Files...)
		}
	}

	if totalFiles > 0 {
		for response := range fChannel {
			query.Files = append(query.Files, response)
		}
	}
	//wg.Done()
	result <- query
}

// SAVE
func grReadDirOld(DirPath string, parent string, result chan Query, wg *sync.WaitGroup) {
	defer wg.Done() // Done at the end, ofc

	files, err := ioutil.ReadDir(DirPath)
	if err != nil {
		fmt.Println("Error opening file:", err)
		result <- (Query{})

		return // Do not continue
	}

	// Preparing the final query
	query := Query{}
	query.Files = make([]FileDescription, 0)

	// Preparing space for routines
	totalLocks := 0
	qChannel := make(chan Query, len(files))
	var thisWaitingGroup sync.WaitGroup

	for _, f := range files {

		if f.IsDir() == false {
			name := parent + "/" + f.Name()
			cmts := ReadFile(DirPath + "\\" + f.Name())

			descriptor := FileDescription{}
			descriptor.Name = name
			descriptor.Comments = cmts

			query.Files = append(query.Files, descriptor)
		} else {
			thisWaitingGroup.Add(1)
			totalLocks++
			go grReadDir(DirPath+"\\"+f.Name(), parent+"/"+f.Name(), qChannel, &thisWaitingGroup)
		}
	}

	thisWaitingGroup.Wait()
	//defer thisWaitingGroup.Wait()
	close(qChannel)

	if totalLocks > 0 {
		for response := range qChannel {
			query.Files = append(query.Files, response.Files...)
		}
	}

	//wg.Done()
	result <- query
}
