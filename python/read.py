import re

def readfile(filename):
    with open(filename, 'r') as f:
        line = f.readline()
        #5 videos, 2 endpoints, 4 request descriptions, 3 caches 100MB each.
        num_vids, num_endpoints, num_requests, num_caches, cache_size = line.split(' ')
        num_vids, num_endpoints, num_requests, num_caches, cache_size = \
        int(num_vids), int(num_endpoints), int(num_requests), int(num_caches),\
            int(cache_size)
        print(num_vids, num_endpoints, num_requests, num_caches, cache_size)

        #Videos 0, 1, 2, 3, 4 have sizes 50MB, 50MB, 80MB, 30MB, 110MB.
        vid_sizes = f.readline().split(' ')
        print(vid_sizes)

        #Endpoint 0 has 1000ms datacenter latency and is connected to 3 caches:
        #The latency (of endpoint 0) to cache 0 is 100ms.
        #The latency (of endpoint 0) to cache 2 is 200ms.
        #The latency (of endpoint 0) to cache 1 is 200ms.
        #Endpoint 1 has 500ms datacenter latency and is not connected to a cache.
        #1500 requests for video 3 coming from endpoint 0.
        #1000 requests for video 0 coming from endpoint 1.
        #500 requests for video 4 coming from endpoint 0.
        #1000 requests for video 1 coming from endpoint 0.

if __name__ == '__main__':
    print(readfile('me_at_the_zoo.in'))

