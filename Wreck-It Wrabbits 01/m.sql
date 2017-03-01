copy (
select ep_id, cache_id, video_id, trunc(score), size
from (
  select *, rank() over (partition by ep_id, video_id order by score desc) rnk
  from (
    select r.ep_id, c.cache_id, r.video_id, v.size, (r.num_requests * (ep.latency - c.latency)) score
    from (
      select video_id, ep_id, sum(num_requests) num_requests
      from requests 
      group by video_id, ep_id
    ) r      
    join ep
      on ep.id = r.ep_id
    join cache c
      on c.ep_id = r.ep_id
    join video v
      on v.id = r.video_id
  ) a
) b
order by rnk, score desc
) to '/tmp/res'
