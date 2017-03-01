#! /bin/bash

rm *.in.out
rm m-sql.zip
zip m-sql.zip p2.pl p.pl c.sql m.sql r.sh

echo "me_at_the_zoo"
psql -U dev -f c.sql -q hc
./p.pl me_at_the_zoo.in > t.sql
psql -U dev -f t.sql -q hc
psql -U dev -f m.sql -q hc
./p2.pl me_at_the_zoo.in > me_at_the_zoo.in.out

echo "trending"
psql -U dev -f c.sql -q hc
./p.pl trending_today.in > t.sql
psql -U dev -f t.sql -q hc
psql -U dev -f m.sql -q hc
./p2.pl trending_today.in > trending_today.in.out

echo "videos"
psql -U dev -f c.sql -q hc
./p.pl videos_worth_spreading.in > t.sql
psql -U dev -f t.sql -q hc
psql -U dev -f m.sql -q hc
./p2.pl videos_worth_spreading.in > videos_worth_spreading.in.out

echo "kittens"
psql -U dev -f c.sql -q hc 
./p.pl kittens.in > t.sql
psql -U dev -f t.sql -q hc
psql -U dev -f m.sql -q hc
./p2.pl kittens.in > kittens.in.out
