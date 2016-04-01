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

\ create pattern $8 c, $C c, $4 c, $6 c, $2 c, $A c,
\ 6 constant pattern#
\ create pattern $8 c, $0 c, $0 c, $0 c, $4 c, $0 c, $0 c, $0 c, $2 c, $0 c, $0 c, $0 c,
\ 3 9 + constant pattern#

create pattern $8 c, $4 c, $2 c,
\ create pattern $6 c, $A c, $C c,
3 constant pattern#

0 variable pos

50000 variable delay
5000 variable md \ min-delay
100 variable dsd \ delay step divider
dsd @ variable cdsd \ current dsd
10 variable ds \ delay-step

\ fraction minimizer 9/10 of teh 3 prior steps
: fracmin ( n1 -- n2 )
    9 * 10 / md @ max ;

: submin ( -- n )
    cdsd @ 1- dup cdsd ! 0= if
        dsd @ cdsd ! \ reset cdsd
        ds @ - md @ max \ update md
    then ;

' fracmin variable minimizer

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
    init
    init-delay
    init-motor
    ['] istep irq-timer0a !
    50000 Timer0A_Init \ 50 ms
    \ endless
    \ 0 PORTF_DATA !
    ;

: ta timer0a_init ;
