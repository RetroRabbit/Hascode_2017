package main

import (
	"bufio"
	"flag"
	"fmt"
	"math"
	"os"
	"path/filepath"
	"sort"
	"strconv"
	"strings"
)

var filename = flag.String("f", "me_at_the_zoo.in", "file")

type Endpoint struct {
	Datacenter int
	caches     map[int]int
}

type Request struct {
	number int
	from   int
	video  int
}

type Video struct {
	number    int
	size      int
	score     map[int]int
	bestScore int
}

type Cache struct {
	number int
	filled int
	videos []Video
}

func main() {
	flag.Parse()
	fmt.Println("hello", *filename)
	file, err := os.Open(*filename)
	if err != nil {
		panic(err)
	}
	fileReader := bufio.NewReader(file)

	numbersLine, _ := fileReader.ReadString('\n')
	numbersLine = numbersLine[:len(numbersLine)-1]

	split := strings.Split(numbersLine, " ")
	numVideo, _ := strconv.Atoi(split[0])
	numEndpoint, _ := strconv.Atoi(split[1])
	numRequests, _ := strconv.Atoi(split[2])
	numCaches, _ := strconv.Atoi(split[3])
	numCacheSize, _ := strconv.Atoi(split[4])

	videoLine, _ := fileReader.ReadString('\n')
	videoLine = videoLine[:len(videoLine)-1]
	split = strings.Split(videoLine, " ")

	Videos := make([]Video, numVideo)
	TotalVideoSize := 0.0
	for v := range Videos {
		Videos[v].size, _ = strconv.Atoi(split[v])
		Videos[v].number = v
		Videos[v].score = make(map[int]int)
		TotalVideoSize += float64(Videos[v].size)
	}

	endpoints := make(map[int]Endpoint)
	for endP := 0; endP < numEndpoint; endP++ {
		endpointLine, _ := fileReader.ReadString('\n')
		endpointLine = endpointLine[:len(endpointLine)-1]
		split = strings.Split(endpointLine, " ")
		endpoint := Endpoint{caches: make(map[int]int)}
		endpoint.Datacenter, _ = strconv.Atoi(split[0])
		numCacheAccess, _ := strconv.Atoi(split[1])

		for k := 0; k < numCacheAccess; k++ {
			endpointCacheLine, _ := fileReader.ReadString('\n')
			endpointCacheLine = endpointCacheLine[:len(endpointCacheLine)-1]
			split = strings.Split(endpointCacheLine, " ")

			cache, _ := strconv.Atoi(split[0])
			latency, _ := strconv.Atoi(split[1])
			endpoint.caches[cache] = latency
		}
		endpoints[endP] = endpoint
	}
	requests := make([]Request, numRequests)
	for r := range requests {
		endpointCacheLine, _ := fileReader.ReadString('\n')
		endpointCacheLine = endpointCacheLine[:len(endpointCacheLine)-1]

		split = strings.Split(endpointCacheLine, " ")
		video, _ := strconv.Atoi(split[0])
		endpoint, _ := strconv.Atoi(split[1])
		numbers, _ := strconv.Atoi(split[2])
		requests[r] = Request{number: numbers, from: endpoint, video: video}
	}
	TotalcacheSize := float64(numCaches * numCacheSize)
	caches := make([]Cache, 0)

	fmt.Println("That one stat", (TotalVideoSize-TotalcacheSize)/TotalcacheSize)
	for c := 0; c < numCaches; c++ {
		cache := Cache{number: c, videos: make([]Video, 0)}

		for r := range requests {
			if cache, ok := endpoints[requests[r].from].caches[c]; ok {
				/*
					currentVideo := &Videos[requests[r].video]
					SizeWeight := (max((TotalVideoSize-TotalcacheSize)/TotalcacheSize, 1) * currentVideo.size)
					TimeSaved := endpoints[requests[r].from].Datacenter - cache
					numberofrequests := requests[r].number

					currentVideo.score += (TimeSaved * numberofrequests) / SizeWeight
				*/
				currentVideo := &Videos[requests[r].video]
				SizeWeight := 1 + (math.Max((TotalVideoSize-TotalcacheSize)/TotalcacheSize, 0) * float64(currentVideo.size))

				TimeSaved := endpoints[requests[r].from].Datacenter - cache
				numberofrequests := requests[r].number

				currentVideo.score[c] += int((math.Pow(float64(numberofrequests*TimeSaved), 2) / math.Pow(float64(endpoints[requests[r].from].Datacenter), 2)) / (SizeWeight) * (1 / float64(len(endpoints[requests[r].from].caches)*10000)))
			}
		}
		caches = append(caches, cache)
	}
	firstMethod := true
	if firstMethod {
		//Calculate best scores
		for v := range Videos {
			for c := range Videos[v].score {
				if Videos[v].bestScore < Videos[v].score[c] {
					Videos[v].bestScore = Videos[v].score[c]
				}
			}
		}
		//Add the videos first
		sort.Sort(&Sorter{Videos, -1})

		for v := range Videos {

			vid := Videos[v]
			for c := range vid.score {
				if vid.score[c] == vid.bestScore {
					cache := &caches[c]

					if cache.filled+Videos[v].size < numCacheSize {
						cache.videos = append(cache.videos, Videos[v])
						cache.filled += Videos[v].size
					}
				}
			}
		}
		//The the caches
		for c := 0; c < numCaches; c++ {
			cache := &caches[c]
			sort.Sort(&Sorter{Videos, c})
			//fmt.Println(Videos)
			for v := range Videos {
				if cache.filled+Videos[v].size < numCacheSize {
					contains := false
					for inv := range cache.videos {
						if cache.videos[inv].number == Videos[v].number {
							contains = true
						}
					}
					if !contains {

						cache.videos = append(cache.videos, Videos[v])
						cache.filled += Videos[v].size
					}
				}
			}

		}
	} else {
		thelist := make([]ELM, 0)
		for c := 0; c < numCaches; c++ {

			for v := range Videos {
				thelist = append(thelist, ELM{v, c, Videos[v].score[c]})
			}
		}
		sort.Sort(&ListSorter{thelist})

		fmt.Println(thelist[:10])
		for k := range thelist[:10] {
			fmt.Println(Videos[k].size)
		}
		for _, l := range thelist {
			cache := &caches[l.c]
			if cache.filled+Videos[l.v].size < numCacheSize {
				cache.videos = append(cache.videos, Videos[l.v])
				cache.filled += Videos[l.v].size
			}
		}
	}

	//Output this sucker
	outFile, err := os.Create(filepath.Base(*filename) + ".out")
	if err != nil {
		panic(err)
	}
	fmt.Fprintln(outFile, len(caches))
	for c := 0; c < numCaches; c++ {
		fmt.Fprint(outFile, c)
		for v := range caches[c].videos {
			fmt.Fprint(outFile, " ", caches[c].videos[v].number)
		}
		fmt.Fprintln(outFile)
	}
}

type Sorter struct {
	v []Video
	c int
}

func (s *Sorter) Len() int {
	return len(s.v)
}

// Swap is part of sort.Interface.
func (s *Sorter) Swap(i, j int) {
	s.v[i], s.v[j] = s.v[j], s.v[i]
}

// Less is part of sort.Interface. It is implemented by calling the "by" closure in the sorter.
func (s *Sorter) Less(i, j int) bool {
	if s.c == -1 {
		return s.v[i].bestScore > s.v[j].bestScore
	}
	return s.v[i].score[s.c] > s.v[j].score[s.c]
}

func max(i, j int) int {
	return int(math.Max(float64(i), float64(j)))
}

type ELM struct {
	v     int
	c     int
	score int
}

type ListSorter struct {
	list []ELM
}

func (s *ListSorter) Len() int {
	return len(s.list)
}

// Swap is part of sort.Interface.
func (s *ListSorter) Swap(i, j int) {
	s.list[i], s.list[j] = s.list[j], s.list[i]
}

// Less is part of sort.Interface. It is implemented by calling the "by" closure in the sorter.
func (s *ListSorter) Less(i, j int) bool {
	return s.list[i].score > s.list[j].score

}
