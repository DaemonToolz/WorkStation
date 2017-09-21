package main

type Query struct {
	Files []FileDescription `json:"files"`
}

type FileDescription struct {
	Name     string   `json:"Name"`
	Comments Comments `json:"Root"`
}

type Root struct {
	Root Comments `xml:"root", json:"Root"`
}

type Comments struct {
	Comments []Comment `xml:"comments", json:"Comments"`
}

type Comment struct {
	Content []Content `xml:"comment", json:"Comment"`
}

type Content struct {
	Author  string `xml:"author", json:"Author"`
	Date    string `xml:"date", json:"Date"`
	Content string `xml:"content", json:"Content""`
}
