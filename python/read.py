import re

def readfile(filename):
    with open(filename, 'r') as f:
        #5 videos, 2 endpoints, 4 request descriptions, 3 caches 100MB each.
        num_vids, num_endpoints, num_request_desc, num_caches, cache_size = [int(x) for x in next(f).split()] # read first line

        #Videos 0, 1, 2, 3, 4 have sizes 50MB, 50MB, 80MB, 30MB, 110MB.
        vid_sizes = [int(x) for x in next(f).split()]
        
        endpoints = []
        for e in xrange(num_endpoints):
            endpoint = {}
            L_d, C = [int(x) for x in next(f).split()]
            caches = {}
            for c in xrange(C):
                cache_id, L_c = next(f).split()
                caches[cache_id] = L_c
            endpoint = {
                'L_d': L_d,
                'C': C,
                'caches': caches
            }
            endpoints.append(endpoint)

        
        request_descriptions = []
        for v in xrange(num_request_desc):
            vid, endpoint_index, number_of_requests = [int(x) for x in next(f).split()]
            request_descriptions.append({
                'vid_index': vid,
                'endpoint_index': endpoint_index,
                'num_r': number_of_requests
            })
        
        return num_vids, num_endpoints, num_request_desc, num_caches, cache_size, vid_sizes, endpoints, request_descriptions
        #Endpoint 0 has 1000ms datacenter latency and is connected to 3 caches:
        #The latency (of endpoint 0) to cache 0 is 100ms.
        #The latency (of endpoint 0) to cache 2 is 200ms.
        #The latency (of endpoint 0) to cache 1 is 200ms.
        #Endpoint 1 has 500ms datacenter latency and is not connected to a cache.
        #1500 requests for video 3 coming from endpoint 0.
        #1000 requests for video 0 coming from endpoint 1.
        #500 requests for video 4 coming from endpoint 0.
        #1000 requests for video 1 coming from endpoint 0.


def output(filename, num_cache, caches):
    with open(filename, 'w') as f:
        # output num cache
        f.write(str(num_cache) + '\n')
        for cache in caches:
            # output cache id
            line = ' '.join([str(cache['id'])] + [ str(c) for c in cache['videos']])
            #line = str(cache['id']) + ' ' + cache['videos'] + '\n'
            f.write(line + '\n')

#num_vids, num_endpoints, num_request_desc, num_caches, cache_size, vid_sizes, endpoints, request_descriptions = readfile('me_at_the_zoo.in')
#output('test.out', 3, [{'id': 0, 'videos': [1,2]},{'id': 1, 'videos': [1,2]},{'id': 3, 'videos': [1,2]}])
