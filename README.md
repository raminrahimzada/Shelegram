# I personally use this to access bash of my RSPI5 device <br/>
It contains some shortcuts like `/cputemp` which replaces long commands and easy to check quickly <br/>
It contains my telegram user id hardcoded inside which only accepts me as a commander<br/>
You can create new bot from bothfather and feed the token to this program like <br/>

```sh
sudo nohup ./shelegram "<your telegram token here>" &
```

Main idea is to replace the global ip thing and also saves you to open ssh port to internet <br/>
I know it is not functional like ssh but handy in some cases
