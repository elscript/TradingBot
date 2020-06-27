select Timestamp, [Close] - [Open] as delta, Volume, ([Close] - [Open]) * Volume as growth
from Candles
order by growth, Timestamp

delete 
from Positions


alter TABLE Positions
add StopLossPrice DECIMAL(20, 2);