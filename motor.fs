cold

compiletoflash

\ 2 constant led-red
\ 8 constant led-green
\ 4 constant led-blue

\ enable led ports
: init-motor ( -- )
    $E PORTF_DEN !
    $E PORTF_DIR !
    $0 PORTF_DATA ! ;

: off $0 PORTF_DATA ! ;

create pattern $8 c, $C c, $4 c, $6 c, $2 c, $A c,
6 constant pattern#

create colors $FF0000 , $FF0000 , $00FF00 , $00FF00 , $0000FF , $0000FF ,
\ create pattern $8 c, $0 c, $0 c, $0 c, $4 c, $0 c, $0 c, $0 c, $2 c, $0 c, $0 c, $0 c,
\ 3 9 + constant pattern#

\ create pattern $6 c, $A c, $C c,
\ create pattern $8 c, $4 c, $2 c,
\ 3 constant pattern#

0 variable pos

20000 variable delay
2000 variable md \ min-delay
12 variable dsd \ delay step divider
dsd @ variable cdsd \ current dsd
1 variable ds \ delay-step
9900 variable fd \ fractional-delay

0 variable minimizer

\ linear minimizer
: submin ( -- n )
    cdsd @ 1- dup cdsd ! 0= if
        dsd @ cdsd ! \ reset cdsd
        ds @ - md @ max \ update md
    then ;

\ fraction minimizer 9/10 of teh 3 prior steps
: fracmin ( n1 -- n2 )
    \ md reached, check for new parameters
    dup md @ = if
        md @ case
            2000 of \ slower ramp
                1500 md !
                9990 fd !
            endof
            1500 of \ slowest ramp
                1000 md !
                9999 fd !
            endof
            1000 of \ linear speedup
                ['] submin minimizer !
                800 md !
            endof
        endcase
    then

    fd @ * 10000 / md @ max ;


: step ( -- )
    \ increment counter, keep bounds
    pos @ 1+ dup pattern# >= if
        drop 0
        delay @
        minimizer @ execute
        dup delay !
        ta-init
    then
    dup pattern + c@ PORTF_DATA !
    dup colors + @ leds n-leds \ change colors
    pos ! ;

: steps ( n -- )
    0 do
        step
        delay @ ms
    loop ;

: endless
    ." press any key to abort"
    2000 ms
    key-flush
    begin
        step
        delay @ ms
    key? until ;

: istep 
    ta-irq-ack
    step ;


: init
    80MHz
    init-delay
    init-ws
    init-motor
    ['] fracmin minimizer !
    ['] istep irq-timer0a !
    50000 Timer0A_Init \ 50 ms
    \ endless
    \ 0 PORTF_DATA !
    ;

\ helper to investigate startups
: d delay @ . ;
