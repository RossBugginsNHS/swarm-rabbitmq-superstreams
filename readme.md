# Docker Swarm Super Streams Example

## Showing:

- RabbitMQ
- SuperStreams
- SingleActiveConsumer
- dotnet rabbitmq streaming client library
- docker swarm scaling
- multiple available consumer containers 

## Summary
This example code shows multiple consumers running, with a producer sending messages for a number of different "customers".

The messages for each customer will be processed in order on the same host. If a host goes down, or the number of consumers is increased, the host processing the messages for a consumer will change.

By default there are 30 stream partitions configured, 10 customer numbers, and 2 consumers.

Each consumer writes the messages it receives to a single analytics API service that then shows what consumers have handled what.

The log output of the analytics service shows for each customer, what hosts have handled its messages, and the last 10 messages handled by the host.

## Quick Start

If a swarm is not already started

```
docker swarm init
```

Build docker files and deploy to local swarm

```
sudo chmod +x ./run.sh
./run.sh
```

## Example Output 1

This is running with 30 stream partitions, 10 customers, and 2 consumers. No changes to the number of consumers was made during the run.

The output shows that the messages for each customer were processed in order by the same host, and each hosts handled a subset of total number of customers.   

The message numbers are generated by the producer, and can be thought of a global producer messageId - per producer. In brackets are the time the consumer received the message.

The 10 most recent messages are shown for each customer.

### What consumer containers are processing which customers messages 

- Consumer baaf3c4d0426 processing messages for 7 customers
- Consumer 622c9a2d0a10 processing messages for 3 customers

Breakdown:

- Customer 0: 622c9a2d0a10
- Customer 1: baaf3c4d0426
- Customer 2: 622c9a2d0a10
- Customer 3: baaf3c4d0426
- Customer 4: baaf3c4d0426
- Customer 5: baaf3c4d0426
- Customer 6: 622c9a2d0a10
- Customer 7: baaf3c4d0426
- Customer 8: baaf3c4d0426
- Customer 9: baaf3c4d0426


```
| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|  Customer: 0
|  Sources: SwarmSuperStream-5,
|  Host (First message received at 13:52:39): 622c9a2d0a10
|  Messages (181): ...1728(14:08:42), 1755(14:08:55), 1766(14:09:02), 1767(14:09:02), 1780(14:09:08), 1782(14:09:10), 1783(14:09:10), 1797(14:09:17), 1799(14:09:18), 1800(14:09:19),
-   |
|  Customer: 1
|  Sources: SwarmSuperStream-8,
|  Host (First message received at 13:52:44): baaf3c4d0426
|  Messages (164): ...1671(14:08:10), 1707(14:08:30), 1716(14:08:35), 1727(14:08:41), 1741(14:08:49), 1743(14:08:50), 1779(14:09:08), 1786(14:09:11), 1791(14:09:15), 1803(14:09:21),
-   |
|  Customer: 2
|  Sources: SwarmSuperStream-13,
|  Host (First message received at 13:52:40): 622c9a2d0a10
|  Messages (199): ...1701(14:08:27), 1706(14:08:30), 1722(14:08:38), 1737(14:08:46), 1745(14:08:51), 1748(14:08:52), 1756(14:08:55), 1770(14:09:03), 1772(14:09:04), 1804(14:09:21),
-   |
|  Customer: 3
|  Sources: SwarmSuperStream-4,
|  Host (First message received at 13:52:46): baaf3c4d0426
|  Messages (174): ...1698(14:08:25), 1699(14:08:25), 1714(14:08:34), 1720(14:08:37), 1750(14:08:52), 1757(14:08:56), 1778(14:09:07), 1784(14:09:10), 1796(14:09:17), 1805(14:09:22),
-   |
|  Customer: 4
|  Sources: SwarmSuperStream-17,
|  Host (First message received at 13:52:49): baaf3c4d0426
|  Messages (169): ...1692(14:08:22), 1713(14:08:33), 1724(14:08:39), 1734(14:08:45), 1739(14:08:48), 1740(14:08:49), 1765(14:09:01), 1775(14:09:05), 1792(14:09:15), 1802(14:09:20),
-   |
|  Customer: 5
|  Sources: SwarmSuperStream-11,
|  Host (First message received at 13:52:36): baaf3c4d0426
|  Messages (196): ...1754(14:08:54), 1759(14:08:57), 1762(14:08:59), 1763(14:08:59), 1764(14:09:00), 1768(14:09:02), 1777(14:09:06), 1788(14:09:13), 1790(14:09:14), 1793(14:09:15),
-   |
|  Customer: 6
|  Sources: SwarmSuperStream-3,
|  Host (First message received at 13:52:37): 622c9a2d0a10
|  Messages (171): ...1703(14:08:28), 1705(14:08:29), 1715(14:08:34), 1730(14:08:43), 1738(14:08:47), 1742(14:08:50), 1747(14:08:51), 1774(14:09:04), 1787(14:09:12), 1794(14:09:15),
-   |
|  Customer: 7
|  Sources: SwarmSuperStream-22,
|  Host (First message received at 13:52:39): baaf3c4d0426
|  Messages (155): ...1710(14:08:32), 1718(14:08:36), 1733(14:08:44), 1746(14:08:51), 1751(14:08:53), 1753(14:08:54), 1760(14:08:58), 1776(14:09:05), 1795(14:09:16), 1806(14:09:22),
-   |
|  Customer: 8
|  Sources: SwarmSuperStream-20,
|  Host (First message received at 13:52:40): baaf3c4d0426
|  Messages (209): ...1731(14:08:43), 1736(14:08:46), 1749(14:08:52), 1769(14:09:03), 1771(14:09:04), 1773(14:09:04), 1781(14:09:09), 1785(14:09:11), 1789(14:09:14), 1801(14:09:19),
-   |
|  Customer: 9
|  Sources: SwarmSuperStream-26,
|  Host (First message received at 13:52:45): baaf3c4d0426
|  Messages (190): ...1660(14:08:04), 1669(14:08:09), 1674(14:08:12), 1717(14:08:36), 1732(14:08:44), 1752(14:08:53), 1758(14:08:56), 1761(14:08:59), 1798(14:09:18), 1807(14:09:23),

```

## Example Output 2

This example shows the number of consumers scaling up and down, and customer  being distributed across available hosts.


### Configuration exert from docker-compose.yml

```
 ...
 consumer:
    deploy:
      replicas: 3
 ...

 ...
  producer:
    environment:
      - SwarmSuperStreams__NumberOfCustomers=3
...
```

### This starts with 3 customers and 3 consumers.

Each customer is being handled by a different host:
- Customer 0: started on dd18795549d9
- Customer 1: started on 6de260a677fd
- Customer 2: started on 0f7159c723f7

```

|
| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (3): 4(14:19:23), 5(14:19:24), 7(14:19:25),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (4): 1(14:19:21), 2(14:19:22), 3(14:19:23), 6(14:19:24),

```

### And then 0 consumers

```
docker service scale superstack_consumer=0
```

One customer did move to a new host, before they were all terminated.
- Customer 0: was last on dd18795549d9
- Customer 1: moved from 6de260a677fd to dd18795549d9 as 6de260a677fd was shutdown
- Customer 2: was last on 0f7159c723f7

```

| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (10): 4(14:19:23), 5(14:19:24), 7(14:19:25), 8(14:19:25), 11(14:19:27), 12(14:19:28), 13(14:19:29), 21(14:19:32), 34(14:19:40), 36(14:19:41),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|       Host (First message received at 14:19:27): dd18795549d9
|       Messages (15): ...19(14:19:31), 20(14:19:32), 23(14:19:34), 25(14:19:34), 26(14:19:35), 28(14:19:36), 30(14:19:37), 31(14:19:38), 38(14:19:42), 41(14:19:43),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (16): ...22(14:19:33), 24(14:19:34), 27(14:19:36), 29(14:19:37), 32(14:19:38), 33(14:19:39), 35(14:19:40), 37(14:19:41), 39(14:19:42), 40(14:19:43),

```

### It is then scaled to 1 consumer

```
docker service scale superstack_consumer=1
```

Each customer is being handled by the same host:
- Customer 0:  moved from dd18795549d9 to the new single consumer of 0ad39aca1229
- Customer 1:  moved from dd18795549d9 to the new single consumer of 0ad39aca1229
- Customer 2:  moved from 0f7159c723f7 to the new single consumer of 0ad39aca1229

```
qj3h4k@CB-004164621257    | info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (10): 4(14:19:23), 5(14:19:24), 7(14:19:25), 8(14:19:25), 11(14:19:27), 12(14:19:28), 13(14:19:29), 21(14:19:32), 34(14:19:40), 36(14:19:41),
|       Host (First message received at 14:20:29): 0ad39aca1229
|       Messages (2): 125(14:20:29), 130(14:20:32),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|       Host (First message received at 14:19:27): dd18795549d9
|       Messages (15): ...19(14:19:31), 20(14:19:32), 23(14:19:34), 25(14:19:34), 26(14:19:35), 28(14:19:36), 30(14:19:37), 31(14:19:38), 38(14:19:42), 41(14:19:43),
|       Host (First message received at 14:20:27): 0ad39aca1229
|       Messages (4): 120(14:20:27), 122(14:20:28), 126(14:20:30), 131(14:20:33),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (16): ...22(14:19:33), 24(14:19:34), 27(14:19:36), 29(14:19:37), 32(14:19:38), 33(14:19:39), 35(14:19:40), 37(14:19:41), 39(14:19:42), 40(14:19:43),
|       Host (First message received at 14:20:26): 0ad39aca1229
|       Messages (7): 119(14:20:26), 121(14:20:27), 123(14:20:28), 124(14:20:29), 127(14:20:31), 128(14:20:31), 129(14:20:31),

```



### And then back to 3 consumers

```
docker service scale superstack_consumer=3
```


- Customer 0:  moved from 0ad39aca1229 to the new consumer 262a01fec68f
- Customer 1:  moved from 0ad39aca1229 to same consumer that 0 is on 262a01fec68f
- Customer 2:  moved from 0ad39aca1229 to the new consumer 2d2337d09fdb

Unsure why customer 0 and 1 stayed on the same host, as there were 3 consumer hosts running at this point.
```

| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (10): 4(14:19:23), 5(14:19:24), 7(14:19:25), 8(14:19:25), 11(14:19:27), 12(14:19:28), 13(14:19:29), 21(14:19:32), 34(14:19:40), 36(14:19:41),
|       Host (First message received at 14:20:29): 0ad39aca1229
|       Messages (16): ...146(14:20:42), 153(14:20:47), 154(14:20:48), 155(14:20:48), 156(14:20:49), 157(14:20:50), 158(14:20:51), 159(14:20:51), 163(14:20:52), 164(14:20:53),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (30): ...226(14:21:25), 234(14:21:29), 237(14:21:31), 239(14:21:32), 242(14:21:33), 245(14:21:34), 248(14:21:36), 250(14:21:37), 253(14:21:38), 254(14:21:39),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|       Host (First message received at 14:19:27): dd18795549d9
|       Messages (15): ...19(14:19:31), 20(14:19:32), 23(14:19:34), 25(14:19:34), 26(14:19:35), 28(14:19:36), 30(14:19:37), 31(14:19:38), 38(14:19:42), 41(14:19:43),
|       Host (First message received at 14:20:27): 0ad39aca1229
|       Messages (19): ...141(14:20:39), 145(14:20:41), 147(14:20:42), 148(14:20:43), 149(14:20:44), 151(14:20:46), 160(14:20:51), 162(14:20:52), 165(14:20:53), 166(14:20:54),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (34): ...244(14:21:35), 246(14:21:35), 247(14:21:36), 249(14:21:36), 252(14:21:37), 255(14:21:39), 257(14:21:40), 259(14:21:42), 260(14:21:43), 263(14:21:45),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (16): ...22(14:19:33), 24(14:19:34), 27(14:19:36), 29(14:19:37), 32(14:19:38), 33(14:19:39), 35(14:19:40), 37(14:19:41), 39(14:19:42), 40(14:19:43),
|       Host (First message received at 14:20:26): 0ad39aca1229
|       Messages (15): ...128(14:20:31), 129(14:20:31), 136(14:20:36), 142(14:20:40), 144(14:20:41), 150(14:20:45), 152(14:20:46), 161(14:20:52), 167(14:20:54), 168(14:20:55),
|       Host (First message received at 14:20:58): 2d2337d09fdb
|       Messages (32): ...233(14:21:29), 235(14:21:30), 238(14:21:31), 240(14:21:32), 251(14:21:37), 256(14:21:40), 258(14:21:41), 261(14:21:44), 262(14:21:45), 264(14:21:46),

```

### Scaled to 4 available consumers

```
docker service scale superstack_consumer=4
```

- Customer 0:  moved from 262a01fec68f to the new consumer 2d2337d09fdb
- Customer 1:  stayed on host 262a01fec68f
- Customer 2:  stayed on host 2d2337d09fdb

Customer 0 moved, but not onto the consumer that was totally free.

```

| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (10): 4(14:19:23), 5(14:19:24), 7(14:19:25), 8(14:19:25), 11(14:19:27), 12(14:19:28), 13(14:19:29), 21(14:19:32), 34(14:19:40), 36(14:19:41),
|       Host (First message received at 14:20:29): 0ad39aca1229
|       Messages (16): ...146(14:20:42), 153(14:20:47), 154(14:20:48), 155(14:20:48), 156(14:20:49), 157(14:20:50), 158(14:20:51), 159(14:20:51), 163(14:20:52), 164(14:20:53),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (342): ...1188(14:30:21), 1193(14:30:23), 1199(14:30:27), 1200(14:30:28), 1203(14:30:30), 1204(14:30:30), 1207(14:30:32), 1213(14:30:36), 1214(14:30:36), 1218(14:30:38),
|       Host (First message received at 14:30:42): 2d2337d09fdb
|       Messages (14): ...1229(14:30:45), 1233(14:30:47), 1234(14:30:48), 1235(14:30:49), 1239(14:30:50), 1241(14:30:51), 1244(14:30:51), 1254(14:30:58), 1259(14:31:01), 1277(14:31:10),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|       Host (First message received at 14:19:27): dd18795549d9
|       Messages (15): ...19(14:19:31), 20(14:19:32), 23(14:19:34), 25(14:19:34), 26(14:19:35), 28(14:19:36), 30(14:19:37), 31(14:19:38), 38(14:19:42), 41(14:19:43),
|       Host (First message received at 14:20:27): 0ad39aca1229
|       Messages (33): ...1250(14:30:55), 1251(14:30:55), 1253(14:30:57), 1255(14:30:58), 1258(14:31:00), 1266(14:31:05), 1267(14:31:05), 1269(14:31:06), 1273(14:31:08), 1274(14:31:09),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (353): ...1192(14:30:23), 1195(14:30:25), 1196(14:30:25), 1197(14:30:26), 1198(14:30:27), 1205(14:30:31), 1209(14:30:34), 1210(14:30:34), 1215(14:30:36), 1217(14:30:38),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (16): ...22(14:19:33), 24(14:19:34), 27(14:19:36), 29(14:19:37), 32(14:19:38), 33(14:19:39), 35(14:19:40), 37(14:19:41), 39(14:19:42), 40(14:19:43),
|       Host (First message received at 14:20:26): 0ad39aca1229
|       Messages (15): ...128(14:20:31), 129(14:20:31), 136(14:20:36), 142(14:20:40), 144(14:20:41), 150(14:20:45), 152(14:20:46), 161(14:20:52), 167(14:20:54), 168(14:20:55),
|       Host (First message received at 14:20:58): 2d2337d09fdb
|       Messages (386): ...1262(14:31:03), 1263(14:31:03), 1264(14:31:04), 1265(14:31:04), 1268(14:31:06), 1270(14:31:06), 1271(14:31:07), 1272(14:31:08), 1275(14:31:09), 1276(14:31:09),

```

### Scaled to 6 available consumers

```
docker service scale superstack_consumer=6
```

- Customer 0:  moved from 2d2337d09fdb to the new consumer 4a48f653aab4
- Customer 1:  stayed on host 262a01fec68f
- Customer 2:  stayed on host 2d2337d09fdb

Note - This now caused the rebalance and all customers were put on different consumer hosts.

```

| info: SuperStreamClients.Analytics.AnalyticsBackgroundWorker[0]
|       Customer: 0
|       Sources: SwarmSuperStream-5,
|       Host (First message received at 14:19:23): dd18795549d9
|       Messages (10): 4(14:19:23), 5(14:19:24), 7(14:19:25), 8(14:19:25), 11(14:19:27), 12(14:19:28), 13(14:19:29), 21(14:19:32), 34(14:19:40), 36(14:19:41),
|       Host (First message received at 14:20:29): 0ad39aca1229
|       Messages (16): ...146(14:20:42), 153(14:20:47), 154(14:20:48), 155(14:20:48), 156(14:20:49), 157(14:20:50), 158(14:20:51), 159(14:20:51), 163(14:20:52), 164(14:20:53),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (342): ...1188(14:30:21), 1193(14:30:23), 1199(14:30:27), 1200(14:30:28), 1203(14:30:30), 1204(14:30:30), 1207(14:30:32), 1213(14:30:36), 1214(14:30:36), 1218(14:30:38),
|       Host (First message received at 14:30:42): 2d2337d09fdb
|       Messages (677): ...3187(14:48:47), 3194(14:48:51), 3195(14:48:52), 3204(14:48:58), 3207(14:49:00), 3212(14:49:02), 3216(14:49:05), 3222(14:49:07), 3230(14:49:11), 3234(14:49:14),
|       Host (First message received at 14:49:15): 4a48f653aab4
|       Messages (11): ...3241(14:49:16), 3244(14:49:17), 3245(14:49:18), 3247(14:49:19), 3250(14:49:20), 3251(14:49:21), 3252(14:49:22), 3253(14:49:23), 3256(14:49:24), 3258(14:49:25),
|
|       Customer: 1
|       Sources: SwarmSuperStream-8,
|       Host (First message received at 14:19:21): 6de260a677fd
|       Messages (1): 0(14:19:20),
|       Host (First message received at 14:19:27): dd18795549d9
|       Messages (15): ...19(14:19:31), 20(14:19:32), 23(14:19:34), 25(14:19:34), 26(14:19:35), 28(14:19:36), 30(14:19:37), 31(14:19:38), 38(14:19:42), 41(14:19:43),
|       Host (First message received at 14:20:27): 0ad39aca1229
|       Messages (705): ...3217(14:49:05), 3219(14:49:06), 3220(14:49:07), 3221(14:49:07), 3224(14:49:08), 3226(14:49:10), 3229(14:49:11), 3231(14:49:12), 3233(14:49:13), 3235(14:49:14),
|       Host (First message received at 14:20:56): 262a01fec68f
|       Messages (358): ...1205(14:30:31), 1209(14:30:34), 1210(14:30:34), 1215(14:30:36), 1217(14:30:38), 3240(14:49:16), 3242(14:49:16), 3243(14:49:17), 3246(14:49:19), 3254(14:49:23),
|
|       Customer: 2
|       Sources: SwarmSuperStream-13,
|       Host (First message received at 14:19:21): 0f7159c723f7
|       Messages (16): ...22(14:19:33), 24(14:19:34), 27(14:19:36), 29(14:19:37), 32(14:19:38), 33(14:19:39), 35(14:19:40), 37(14:19:41), 39(14:19:42), 40(14:19:43),
|       Host (First message received at 14:20:26): 0ad39aca1229
|       Messages (15): ...128(14:20:31), 129(14:20:31), 136(14:20:36), 142(14:20:40), 144(14:20:41), 150(14:20:45), 152(14:20:46), 161(14:20:52), 167(14:20:54), 168(14:20:55),
|       Host (First message received at 14:20:58): 2d2337d09fdb
|       Messages (1015): ...3225(14:49:09), 3227(14:49:10), 3228(14:49:10), 3232(14:49:12), 3236(14:49:14), 3237(14:49:15), 3248(14:49:19), 3249(14:49:20), 3255(14:49:23), 3257(14:49:24),

```

## Configuration

The defaults for the configuration can be seen in RabbitMqStreamOptions.cs

```
public class RabbitMqStreamOptions
{
    public static string Name = "SwarmSuperStreams";
    public string HostName { get; set; } = "rabbitmq";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int StreamPort { get; set; } = 5552;
    public string StreamName { get; set; } = "SwarmSuperStream";
    public int RandomSeed { get; set; } = 5432;
    public int ProducerMessageSendLimit { get; set; } = int.MaxValue;
    public int NumberOfCustomers { get; set; } = 100;
    public int ProducerSendDelayMin { get; set; } = 100;
    public int ProducerSendDelayMax { get; set; } = 1000;
    public string ConsumerAppReference { get; set; } = "SwarmSuperStreamApp";
    public int ConsumerHandleDelayMin { get; set; } = 100;
    public int ConsumerHandleDelayMax { get; set; } = 1000;
    public int MaxMessagesToShowInAnalytics {get;set;} = 10;
    public bool Consumer {get;set;} = true;
    public bool Producer {get;set;} = true;
    public bool Analytics {get;set;} = true;
    public string AnalyticsApi{get;set;}="http://localhost:5070";
}
```

These can be set from Environment Variables, appsettings.json, or inline code.

From docker compose environment variables

```
  producer:
    image: swarmsuperstreamclient
    depends_on:
      - rabbitmq
    environment:
      - SwarmSuperStreams__Consumer=false
      - SwarmSuperStreams__Producer=true
      - SwarmSuperStreams__Analytics=false   
      - SwarmSuperStreams__NumberOfCustomers=10
      - Logging__LogLevel__Default=Information
      - DOTNET_ENVIRONMENT=Production
    deploy:
```

App Settings

```
{
  "SwarmSuperStreams": {
    "StreamName": "SwarmSuperStream",
    "NumberOfCustomers": 10,
    "HostName": "localhost"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "SuperStreamClients": "Warning",
      "SuperStreamClients.Analytics": "Information"
    }
  }
}
```

Inline code

```
builder.Services.AddSwarmSuperStream(
    builder.Configuration.GetSection(RabbitMqStreamOptions.Name),
    options =>
    {
        options.ConsumerHandleDelayMin = 100;
    });
```



### Example Config Chamges

[TODO]

#### Change Number of Customers

[TODO]

## API

### Swagger Exposed

```
http://localhost:5070/swagger/index.html
```

### Basic Summary

```
curl -X 'GET' \
  'http://localhost:5070/CustomerAnalytics' \
  -H 'accept: text/plain'
```

#### Example Response

```
{
  "activeCustomerCount": 10,
  "activeConsumerCount": 5,
  "hostSummaries": [
    {
      "active": true,
      "hostName": "df53fdf56b27",
      "activeCustomerCount": 3,
      "allTimeCustomerCount": 3
    },
    {
      "active": true,
      "hostName": "79514938316f",
      "activeCustomerCount": 2,
      "allTimeCustomerCount": 2
    },
    {
      "active": true,
      "hostName": "bed10d676f4e",
      "activeCustomerCount": 2,
      "allTimeCustomerCount": 4
    },
    {
      "active": true,
      "hostName": "ecd162c73b24",
      "activeCustomerCount": 2,
      "allTimeCustomerCount": 2
    },
    {
      "active": true,
      "hostName": "220de3e8239d",
      "activeCustomerCount": 1,
      "allTimeCustomerCount": 5
    },
    {
      "active": false,
      "hostName": "4270e0ea09b1",
      "activeCustomerCount": 0,
      "allTimeCustomerCount": 5
    }
  ],
  "customerSummaries": [
    {
      "customerId": 0,
      "differentHostCount": 2,
      "numberOfMessages": 5,
      "lastMessageNumber": 60,
      "lastMessage": "16:30:02.5234524",
      "customerHostSummaries": [
        {
          "customerId": 0,
          "hostId": "220de3e8239d",
          "numberOfMessages": 4,
          "lastMessageNumber": 21,
          "lastMessage": "16:29:39.4594799"
        },
        {
          "customerId": 0,
          "hostId": "bed10d676f4e",
          "numberOfMessages": 1,
          "lastMessageNumber": 60,
          "lastMessage": "16:30:02.5234524"
        }
      ]
    },
    {
      "customerId": 1,
      "differentHostCount": 2,
      "numberOfMessages": 3,
      "lastMessageNumber": 68,
      "lastMessage": "16:30:06.5996182",
      "customerHostSummaries": [
        {
          "customerId": 1,
          "hostId": "4270e0ea09b1",
          "numberOfMessages": 2,
          "lastMessageNumber": 34,
          "lastMessage": "16:29:46.9162434"
        },
        {
          "customerId": 1,
          "hostId": "df53fdf56b27",
          "numberOfMessages": 1,
          "lastMessageNumber": 68,
          "lastMessage": "16:30:06.5996182"
        }
      ]
    },
    {
      "customerId": 2,
      "differentHostCount": 1,
      "numberOfMessages": 5,
      "lastMessageNumber": 42,
      "lastMessage": "16:29:51.6064334",
      "customerHostSummaries": [
        {
          "customerId": 2,
          "hostId": "220de3e8239d",
          "numberOfMessages": 5,
          "lastMessageNumber": 42,
          "lastMessage": "16:29:51.6064334"
        }
      ]
    },
    {
      "customerId": 3,
      "differentHostCount": 2,
      "numberOfMessages": 6,
      "lastMessageNumber": 71,
      "lastMessage": "16:30:08.3471020",
      "customerHostSummaries": [
        {
          "customerId": 3,
          "hostId": "4270e0ea09b1",
          "numberOfMessages": 4,
          "lastMessageNumber": 44,
          "lastMessage": "16:29:52.8437047"
        },
        {
          "customerId": 3,
          "hostId": "df53fdf56b27",
          "numberOfMessages": 2,
          "lastMessageNumber": 71,
          "lastMessage": "16:30:08.3471020"
        }
      ]
    },
    {
      "customerId": 4,
      "differentHostCount": 2,
      "numberOfMessages": 5,
      "lastMessageNumber": 73,
      "lastMessage": "16:30:09.5852261",
      "customerHostSummaries": [
        {
          "customerId": 4,
          "hostId": "220de3e8239d",
          "numberOfMessages": 3,
          "lastMessageNumber": 38,
          "lastMessage": "16:29:49.3138093"
        },
        {
          "customerId": 4,
          "hostId": "ecd162c73b24",
          "numberOfMessages": 2,
          "lastMessageNumber": 73,
          "lastMessage": "16:30:09.5852261"
        }
      ]
    },
    {
      "customerId": 5,
      "differentHostCount": 2,
      "numberOfMessages": 10,
      "lastMessageNumber": 75,
      "lastMessage": "16:30:09.9717926",
      "customerHostSummaries": [
        {
          "customerId": 5,
          "hostId": "220de3e8239d",
          "numberOfMessages": 6,
          "lastMessageNumber": 45,
          "lastMessage": "16:29:53.2744053"
        },
        {
          "customerId": 5,
          "hostId": "79514938316f",
          "numberOfMessages": 4,
          "lastMessageNumber": 75,
          "lastMessage": "16:30:09.9717926"
        }
      ]
    },
    {
      "customerId": 6,
      "differentHostCount": 2,
      "numberOfMessages": 9,
      "lastMessageNumber": 63,
      "lastMessage": "16:30:03.8348368",
      "customerHostSummaries": [
        {
          "customerId": 6,
          "hostId": "220de3e8239d",
          "numberOfMessages": 8,
          "lastMessageNumber": 33,
          "lastMessage": "16:29:46.1844248"
        },
        {
          "customerId": 6,
          "hostId": "df53fdf56b27",
          "numberOfMessages": 1,
          "lastMessageNumber": 63,
          "lastMessage": "16:30:03.8348368"
        }
      ]
    },
    {
      "customerId": 7,
      "differentHostCount": 3,
      "numberOfMessages": 5,
      "lastMessageNumber": 74,
      "lastMessage": "16:30:09.7761565",
      "customerHostSummaries": [
        {
          "customerId": 7,
          "hostId": "4270e0ea09b1",
          "numberOfMessages": 3,
          "lastMessageNumber": 32,
          "lastMessage": "16:29:45.2452400"
        },
        {
          "customerId": 7,
          "hostId": "bed10d676f4e",
          "numberOfMessages": 1,
          "lastMessageNumber": 58,
          "lastMessage": "16:30:01.4495944"
        },
        {
          "customerId": 7,
          "hostId": "ecd162c73b24",
          "numberOfMessages": 1,
          "lastMessageNumber": 74,
          "lastMessage": "16:30:09.7761565"
        }
      ]
    },
    {
      "customerId": 8,
      "differentHostCount": 2,
      "numberOfMessages": 11,
      "lastMessageNumber": 76,
      "lastMessage": "16:30:10.3564480",
      "customerHostSummaries": [
        {
          "customerId": 8,
          "hostId": "4270e0ea09b1",
          "numberOfMessages": 7,
          "lastMessageNumber": 40,
          "lastMessage": "16:29:50.1447528"
        },
        {
          "customerId": 8,
          "hostId": "bed10d676f4e",
          "numberOfMessages": 4,
          "lastMessageNumber": 76,
          "lastMessage": "16:30:10.3564480"
        }
      ]
    },
    {
      "customerId": 9,
      "differentHostCount": 3,
      "numberOfMessages": 6,
      "lastMessageNumber": 70,
      "lastMessage": "16:30:08.2344526",
      "customerHostSummaries": [
        {
          "customerId": 9,
          "hostId": "4270e0ea09b1",
          "numberOfMessages": 4,
          "lastMessageNumber": 43,
          "lastMessage": "16:29:52.2842900"
        },
        {
          "customerId": 9,
          "hostId": "bed10d676f4e",
          "numberOfMessages": 1,
          "lastMessageNumber": 59,
          "lastMessage": "16:30:02.3055585"
        },
        {
          "customerId": 9,
          "hostId": "79514938316f",
          "numberOfMessages": 1,
          "lastMessageNumber": 70,
          "lastMessage": "16:30:08.2344526"
        }
      ]
    }
  ]
}
```

## Run In Swarm
This demo uses docker swarm to show scaling of consumers up and down.

### Pre Reqs

Have docker swarm running 

```
docker swarm init
```

### Run
Set permissions and run

```
sudo chmod +x ./run.sh
./run.sh
````

> If running the run.sh again and there is an error creating the stack. Wait a few seconds and then re run the run.sh command, as it may be taking a while to remove the stack before it is recreated.

### Configuring 
Scale the number of consumers up with:

```
docker service scale superstack_consumer=5
```

Or down to 1 with:

```
docker service scale superstack_consumer=1
```



### View logs
By default, the analytics service logs are shown after the service is running (it will take a little while for rabbitmq to come online before the first output is shown). 

Logs for producer and consumer can be viewed through another terminal.

```
docker service logs superstack_consumer -f
docker service logs superstack_producer -f
```



## Run Locally

The demo can also run in a single app with a local rabbitmq instance. Configuration allow turning consumer/producer/analytics on or off.

Build Pre Configured SuperStream Docker Image
```
docker build ./rabbitmq -t swarmsuperstreamrabbitmq
```

Run Rabbit Docker Image
```
docker run \
--name swarmsuperstreamrabbitmq1 \
-p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15671:15671 -p 15672:15672 -p 15691:15691 -p 15692:15692 -p 25672:25672 -p 5552:5552 \
-e RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS="-rabbitmq_stream advertised_host localhost" \
swarmsuperstreamrabbitmq
```

Run App

```
dotnet run --project src/SuperStreamClients 
```

## Todo / Issues
- SuperStreamAnalytics.cs to tidy
- Metrics api / prometheus
- Web GUI for showing stats / analytics