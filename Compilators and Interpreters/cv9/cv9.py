from tkinter import *
from math import *
#from idlelib.ColorDelegator import prog
import os
from _threading_local import local
import code

NOTHING = 0
NUMBER = 1
WORD = 2
SYMBOL = 3

globalMemory = list()
localMemory = list()

globalNamespace = dict()
localNamespace = dict()
globalVariableAdr = None
localVariableAdr = None
subroutines = dict()

########################################################################################

class Korytnacka:
    
    def __init__(self, canvas, x=300, y=200, heading=0):
        
        self.canvas = canvas
        self.heading = heading
        self.x = x
        self.y = y
        self.input = ''
        self.index = 0
        self.look = ''
        self.token = ''
        self.kind = ''
        self.previousToken = ''
        self.color = 'black'
        #######################
        self.STATE_END = 0
        self.STATE_START = 1
        self.STATE_NUMBER = 2
        self.STATE_WORD = 3
        self.STATE_SYMBOL = 4
        self.STATE_SYMBOLLESS = 5
        self.STATE_SYMBOLLESSEQUAL = 6
        self.STATE_SYMBOLDIFFERENT = 7
        self.STATE_SYMBOLGREATER = 8
        self.STATE_SYMBOLGREATEREQUAL = 9
        self.STATE_SYMBOLDOT = 10
        self.STATE_SYMBOLDOTS = 11
        self.STATE_SYMBOLINTERVAL = 12
        w,h = 13,256
        self.change = [[0 for x in range(w)] for y in range(h)] 
        ###########################

        self.table = [[x] for x in range(16)]
        for i in range(0,15):
            self.table[i] = self.parse_other
        self.table[self.hash("vypis")] = self.parse_print
        self.table[self.hash("dopredu")] = self.parse_forward
        self.table[self.hash("vlavo")] = self.parse_left
        self.table[self.hash("vpravo")] = self.parse_right
##        self.table[self.hash("opakuj")] = self.parse_repeat
##        self.table[self.hash("ak")] = self.parse_if
##        self.table[self.hash("kym")] = self.parse_while
##        self.table[self.hash("definuj")] = self.parse_definition
        

    def hash(self, text):
        suma = 0
        print('hop')
        for i in list(text):
            suma = suma + ord(i)
            #print(suma)
        out = suma % 16
        return out

    
    def table_fill(self):
        for state in range(0,12):
            for znak in range(0,255):
                self.change[state, znak] = self.STATE_END
        self.change[self.STATE_START, ' '] = self.STATE_START        
        self.change[self.STATE_START, '/n'] = self.STATE_START 
        for z in range (48,57):
            znak = ord(z)
            self.change[self.STATE_START, znak] = self.STATE_NUMBER
            self.change[self.STATE_NUMBER, znak] = self.STATE_NUMBER
        for z in range(65,90):
            znak = ord(z)
            self.change[self.STATE_START, znak] = self.STATE_WORD
            self.change[self.STATE_WORD, znak] = self.STATE_WORD
        for z in range(97,122):
            znak = ord(z)
            self.change[self.STATE_START, znak] = self.STATE_WORD
            self.change[self.STATE_WORD, znak] = self.STATE_WORD
        for znak in ('[', ']', '(', ')', '+', '-', '*', '/', '=', ','):
            self.change[self.STATE_START, znak] = self.STATE_SYMBOL
            
        self.change[self.STATE_START, '<'] = self.STATE_SYMBOLLESS 
        self.change[self.STATE_SYMBOLLESS, '='] = self.STATE_SYMBOLLESSEQUAL
        self.change[self.STATE_SYMBOLLESS, '>'] = self.STATE_SYMBOLDIFFERENT
        self.change[self.STATE_START, '>'] = self.STATE_SYMBOLGREATER
        self.change[self.STATE_SYMBOLGREATER, '='] = self.STATE_SYMBOLGREATEREQUAL
        self.change[self.STATE_START, '.'] = self.STATE_SYMBOLDOT 
        self.change[self.STATE_SYMBOLDOT, '.'] = self.STATE_SYMBOLDOTS
        self.change[self.STATE_SYMBOLDOTS, '.'] = self.STATE_SYMBOLINTERVAL
        
    def next(self):
        if self.index >= len(self.input):
            self.look = '\0'
        else:
            self.look = self.input[self.index]
            self.index += 1
 
    def scan(self):
        self.token = ""
        self.state = self.change[self.STATE_START, self.look]
        while self.state != self.STATE_END:
            if self.state != self.STATE_START:
                self.token = self.token + look
            self.next()
            self.state = self.change[self.state, look]


    def interpret(self):
        while True:
            if self.token in ('dopredu', 'dp') :
                self.scan()
                try:
                    self.dopredu(int(self.token))
                except ValueError:
                    raise Exception('Zly vstup: "' + self.token + '"')
                self.scan()
            elif self.token in ('vlavo', 'vl'):
                self.scan()
                try:
                    self.vlavo(int(self.token))
                except ValueError:
                    raise Exception('Zly vstup: "' + self.token + '"')    
                self.scan()
            elif self.token in ('vpravo', 'vp'):
                self.scan()
                try:
                    self.vpravo(int(self.token))
                except ValueError:
                    raise Exception('Zly vstup: "' + self.token + '"')
                self.scan()
            elif self.token in ('opakuj', '*'):
                #print(self.token)
                if self.token == '*':
                    self.count = int(self.previousToken)
                else:
                    self.scan()
                    #print(self.token)
                    try:
                        self.count = int(self.token)
                    except ValueError:
                        raise Exception('Zly vstup: "' + self.token + '"')
                self.scan()
                if self.token == '[':
                    self.scan()
                    start = self.position
                    while self.count > 0:
                        self.index = start
                        self.next()
                        self.scan()
                        self.interpret()
                        self.count -= 1
                if self.token == ']':
                    self.scan()
            elif self.token.isdigit():
                self.scan()
            elif self.position == len(self.input) - 1:
                break
            else:
                raise Exception('Neznamy prikaz: "' + self.token + '"')

    def number(self):
        result = Const(int(self.token))
        self.scan()
        return result


    def operand(self):
        if self.kind == NUMBER:
            result = Const(float(self.token))
            self.scan()
        elif self.kind == WORD:
            if not localNamespace and self.token in localNamespace:
                result = Access(localNamespace[self.token])
            elif self.token in globalNamespace:
                id = globalNamespace[self.token]
                if isinstance(id, Variable):
                    raise Exception('Zla premenna!')
                result = Access(id)
            else:
                raise Exception('eznama premenna!')
            self.scan()
        elif self.token == '(':
            self.scan()
            result = Expression(result)
            if self.token != ')':
                raise Exception('Problem so )!')
            self.scan()
        else:
            raise Exception('Cislo alebo nazov premennej!')
        return result
        
    def minus(self):
        if self.token == '-':
            self.scan()
            return -self.operand()
        elif self.token == '@':
            self.scan()
            return sqrt(self.operand())
        else:
            return self.operand()
    
    def muldiv(self):
        result = self.minus()
        while self.token != '':
            if self.token == '*':
                self.scan()
                result *= self.minus()
            elif self.token == '/':
                self.scan()
                result /= self.minus()
            elif self.token == '^':
                self.scan()
                result = pow(result, self.minus())
            else:
                return result
        return result
    
    def addsub(self):
        result = self.muldiv()
        while self.token != '':
            if self.token == '+':
                self.scan()
                result += self.muldiv()
            elif self.token == '-':
                self.scan()          
                result -= self.muldiv()
            else:
                return result
        return result
    
            
    def compile(self):       
        result = Commands()
        while self.kind == WORD:
            if self.token == 'vypis':
                self.scan()
                result.Add(Print(expr()))
            elif self.token == 'dopredu':
                self.scan()
                result.Add(FD(self.addsub()))
            elif self.token == 'vlavo':
                self.scan()
                result.Add(LT(self.addsub()))
            elif self.token == 'vpravo':
                self.scan()
                result.Add(RT(self.addsub()))
            elif self.token == 'opakuj':
                self.scan()
                count = self.addsub()
                if self.token == '[':
                    self.scan()
                    body = self.compile()
                    if self.token == ']':
                        self.scan()
                    result.Add(Repeat(count, body))
            else:
                return result
            
    def getInput(self, inputt):
        self.input = inputt
        self.index = 0
        self.token = ''
        self.previousToken = ''
        self.look = ''

    def dopredu(self, kolko):
        newX = self.x + cos(radians(self.heading)) * kolko
        newY = self.y + sin(radians(self.heading)) * kolko
        self.canvas.create_line(self.x, self.y, newX, newY, fill=self.color)
        self.x = newX
        self.y = newY
        
    def zmaz(self):
        self.canvas.delete('all')
        self.x = int(self.canvas.cget('width')) / 2
        self.y = int(self.canvas.cget('height')) / 2
        self.heading = -90
        self.color = 'black'
        
    def farba(self, r, g, b):
        if r > 255:
            r = 255
        if g > 255:
            g = 255
        if b > 255:
            b = 255
        self.color = str('#%02x%02x%02x' % (r, g, b))
        
    def bod(self, polomer):
        self.canvas.create_oval(self.x - polomer, self.y - polomer, self.x + polomer, self.y + polomer, fill=self.color, outline=self.color)

    def vpravo(self, uhol):
        self.heading += uhol

    def vlavo(self, uhol):
        self.heading -= uhol

    def kresli(self, text, uhol, krok, zmena):
        for char in text:
            if krok < 1:
                return
            elif char == 'd':
                self.dopredu(krok)
            elif char == 'c':
                self.dopredu(-krok)
            elif char == 'l':
                self.vlavo(uhol)
            elif char == 'p':
                self.vpravo(uhol)
            elif char == '*':
                self.kresli(text, uhol, krok * zmena, zmena)


    def parse(self):
        result = Block()
        while self.kind == WORD:
            if self.token == "vypis":
                self.scan()
                result.add(Print(self.addsub()))
            else:
                name = self.token
                self.scan()
                self.check("SYMBOL", '=')
                self.scan()
                if name in variables:
                    self.variables[name] = 2 + len(self.variables)
                result.add(Assign(Variable(name), addsub()))
        return result
    
        if self.token == "kym":
            self.scan()
            test = expr()
            self.check(SYMBOL, "[")
        elif self.token == "definuj":
            if locals != null:
                raise Exception('Def na zlom mieste!')
            result.add(self.parse_definition())
        else:
            name = self.token
            self.scan()
            if self.token == "=":
                result.add(self.parse_assign(name))
            else:
                result.add(self.parse_call(name))
        return result

    def parse_definition():
        self.scan()
        self.check(self.WORD)
        name = self.token
        if name in globalNamespace:
            raise Exception('token sa uy pouziva')
        self.scan()
        result = Subroutine(name, params(), null)
        globalNamespace[name] = result
        self.check(SYMBOL, "[")
        self.scan()
        localss = result.vars
        localvaradr = -1
        result.body = self.parse()
        localss = null
        self.check(SYMBOL, ']')
        self.scan()
        return result

    def parse_assign(self,name):
        self.scan()
        if localss != null:
            if name in localss:
                result = Assign(localss[name], expr())
            else:
                var = LocalVariable(name,localvaradr)
                result = Assign(var, expr())
                localss[name] = var
                localvaradr = localvaradr -1
        else:
            if name in globalNamespace:
                if type(globalNamespace[name]) != Variable:
                    raise Exception('chyba ',name,' nie je premenna')
                result = Assign(globalNamespace[name], expr())
            else:
                var = GlobalVariable(name, globalvariableadr)
                result =Assign(var, expr())
                globalNamespace[name] = var
                globalvariableadr = globalvariableadr + 1
        return result

    def parse_call(name):
        if name not in globalNamespace:
            raise Exception('chyba neznamy prikaz ',name)
        if type(globalNamespace[name]) != Subroutine:
            raise Exception('chyba prikaz ',name, ' nie je podprogram')
        subr = globalNamespace[name]
        args = Block()
        if token =="(":
            self.scan()
            if token != ")":
                args.add(expr())
                while token == ",":
                    self.scan()
                    args.add(expr())
            self.check('SYMBOL', ")")
            self.scan()
        if len(args.items) != subr.paramcount:
            raise Exception('nespravny pocet parametrov')
        return Call(subr, args)
            

    def parse_print(self, result):
        if token != "vypis":
            self.parse_other(result)
        else:
            self.scan()
            self.result.Add(Print(expr()))

    def parse_forward(self, result):
        if token != "dopredu":
            self.parse_other(result)
        else:
            self.scan()
            self.result.Add(FD(expr()))

    def parse_left(self, result):
        if token != "vlavo":
            self.parse_other(result)
        else:
            self.scan()
            self.result.Add(LT(expr()))

    def parse_right(self, result):
        if token != "vpravo":
            self.parse_other(result)
        else:
            self.scan()
            self.result.Add(RT(expr()))

##    def parse_repeat(self, result):
##        if token != "opakuj":
##            self.parse_other(result)
##        else:
##            self.scan()
##            self.result.Add(RT(expr()))

    def parse_other(self, result):
        name = token
        self.scan()
        if token == "=":
            result.add(self.parse_assign(name))
        else:
            result.Add(self.parse_call(name))
        


###############################################################################
###############################################################################
    
class Machine(Korytnacka):
    
    def __init__(self, canvas):
        self.INSTRUCTION_FD = 1
        self.INSTRUCTION_LT = 2
        self.INSTRUCTION_RT = 3
        self.INSTRUCTION_SET = 4
        self.INSTRUCTION_LOOP = 5

        self.INSTRUCTION_JUMPIFFALSE = 20
        self.INSTRUCTION_CALL = 21
        self.INSTRUCTION_RETURN = 22
        
        self.mem = list()
        self.pc = 0
        self.top = 0
        self.terminated = False
        self.adr = 0
        
        self.turtle = Korytnacka(canvas)
        self.turtle.zmaz()
        
    def reset(self):
        self.pc = 0
        self.terminated = False
        
    def poke(self, code):
        self.mem[self.adr] = code
        self.adr += 1
        
    def execute(self):
        if self.mem[self.pc] == self.INSTRUCTION_FD:
            self.pc += 1
            self.turtle.dopredu(self.mem[self.pc])
            self.pc += 1
        elif self.mem[self.pc] == self.INSTRUCTION_LT:
            self.pc += 1
            self.turtle.vlavo(self.mem[self.pc])
            self.pc += 1
        elif self.mem[self.pc] == self.INSTRUCTION_RT:
            self.pc += 1
            self.turtle.vpravo(self.mem[self.pc])
            self.pc += 1
        elif self.mem[self.pc] == self.INSTRUCTION_SET:
            self.pc += 1
            index = self.mem[self.pc]
            self.pc += 1
            self.mem[index] = self.mem[self.pc]
            self.pc += 1
        elif self.mem[self.pc] == self.INSTRUCTION_LOOP:
            self.pc += 1
            index = self.mem[self.pc]
            self.pc += 1
            self.mem[index] = self.mem[index] - 1
            if self.mem[index] <= 0:
                self.pc += 1
            else:
                self.pc = self.mem[self.pc]
        ###############################################################
        elif self.mem[self.pc] == self.INSTRUCTION_JUMPIFFALSE:
            self.pc += 1
            if self.mem[self.top] ==0:
                self.pc = self.mem[self.pc]
            else:
                self.pc +=1
                self.top +=self.top
        elif self.mem[self.pc] == self.INSTRUCTION_CALL:
            self.pc += 1
            self.top -=1
            self.mem[self.top] += 1
            self.pc = self.mem[self.pc]
        elif self.mem[self.pc] == self.INSTRUCTION_RETURN:
            self.pc = self.mem[self.top]
            self.top +=1  
        ###############################################################
        else:
            self.terminated = True
            
    def run(self):
        self.reset()
        while not self.terminated:
            self.execute()



    

################################################################################
################################################################################

###############################################################################
###############################################################################
    
class Machine1(Korytnacka):
    
    def __init__(self, canvas):
        self.INSTRUCTION_PUSH = 1
        self.INSTRUCTION_MINUS = 2
        self.INSTRUCTION_ADD = 3
        self.INSTRUCTION_SUB = 4
        self.INSTRUCTION_MUL = 5
        self.INSTRUCTION_DIV = 6

        self.INSTRUCTION_GET = 7
        self.INSTRUCTION_SET = 8

        self.INSTRUCTION_PRINT = 9
        self.INSTRUCTION_MUL = 10
        self.INSTRUCTION_FD = 11
        self.INSTRUCTION_LT = 12
        self.INSTRUCTION_RT = 13

        self.INSTRUCTION_JUMP =19

        self.INSTRUCTION_JUMPIFFALSE = 20
        self.INSTRUCTION_CALL = 21
        self.INSTRUCTION_RETURN = 22
        
        self.mem = list()
        self.pc = 0
        self.top = 0
        self.terminated = False
        self.adr = 0
        
        self.turtle = Korytnacka(canvas)
        self.turtle.zmaz()
        
    def reset(self):
        self.pc = 0
        self.top = len(self.mem)
        self.terminated = False
        
    def poke(self, code):
        self.mem[self.adr] = code
        self.adr += 1
        
    def execute(self):
        print("self.pc,", self.pc, " self.top", self.top)
        if self.mem[self.pc] == self.INSTRUCTION_PUSH:
            self.pc += 1
            self.top -=1
            self.mem[self.top] = self.mem[self.pc]
            self.pc += 1
        elif self.mem[self.pc] == self.INSTRUCTION_MINUS:
            self.pc += 1
            self.mem[self.top] = -self.mem[self.top]
        elif self.mem[self.pc] == self.INSTRUCTION_ADD:
            self.pc += 1
            self.mem[self.top+1] = self.mem[self.top + 1] + self.mem[self.top]
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_SUB:
            self.pc += 1
            self.mem[self.top +1] = self.mem[self.top+1] - self.mem[self.top]
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_MUL:
            self.pc += 1
            self.mem[self.top +1] = self.mem[self.top+1] * self.mem[self.top]
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_DIV:
            self.pc += 1
            self.mem[self.top +1] = self.mem[self.top+1] / self.mem[self.top]
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_GET:
            self.pc += 1
            self.index = self.mem[self.pc]
            self.pc += 1
            self.top -=1
            self.mem[self.top] = self.mem[self.index]
        elif self.mem[self.pc] == self.INSTRUCTION_SET:
            self.pc += 1
            self.index = self.mem[self.pc]
            self.pc += 1
            self.mem[self.index] = self.mem[self.top]
            self.top +=1
        elif self.mem[self.pc] == self.INSTRUCTION_PRINT:
            self.pc +=1
            print(self.mem[self.top])
            self.top +=1
        elif self.mem[self.pc] == self.INSTRUCTION_JUMP:
            self.pc = self.mem[self.pc+1]
        elif self.mem[self.pc] == self.INSTRUCTION_FD:
            self.pc += 1
            self.turtle.dopredu(self.mem[self.top])
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_LT:
            self.pc += 1
            self.turtle.vlavo(self.mem[self.top])
            self.top += 1
        elif self.mem[self.pc] == self.INSTRUCTION_RT:
            self.pc += 1
            self.turtle.vpravo(self.mem[self.top])
            self.top += 1
            
        ###############################################################
        elif self.mem[self.pc] == self.INSTRUCTION_JUMPIFFALSE:
            self.pc += 1
            if self.mem[self.top] ==0:
                self.pc = self.mem[self.pc]
            else:
                self.pc +=1
                self.top +=self.top
        elif self.mem[self.pc] == self.INSTRUCTION_CALL:
            self.pc += 1
            self.top -=1
            self.mem[self.top] += 1
            self.pc = self.mem[self.pc]
        elif self.mem[self.pc] == self.INSTRUCTION_RETURN:
            self.pc = self.mem[self.top]
            self.top +=1  
        ###############################################################
        else:
            self.terminated = True
            
    def run(self):
        self.reset()
        while not self.terminated:
            self.execute()



    

################################################################################
################################################################################
            
class Syntax():
    def __init__(self):
        self.counter_adr = 99

    def execute(self):
        pass

    def generate(self, masina):
        pass

class Block(Syntax):
    def __init__(self, zoznam):
        self.items = zoznam
        self.dalsi = len(zoznam)
        ##self.execute()
    
            
    def add(self, item):
        self.items.add(item)
        self.dalsi = len(self.items)
        
    def execute(self, masina):
        for i in range(len(self.items)):
            self.items[i].execute(masina)

    def generate(self, masina):
        for i in range(len(self.items)):
            self.items[i].generate(masina)

            
class While(Syntax):
    def __init__(self, ntest, nbody):
        self.test = ntest
        self.body = Snbody

    def generate(self):
        self.test_adr = self.counter_adr
        self.test.generate()
        masina.poke(self.INSTRUCTION_JUMPIFFALSE)
        self.jump_ins = self.counter_adr
        self.counter_adr +=1
        self.body.generate()
        masina.poke(self.INSTRUCTION_JUMP)
        masina.poke(self.test_adr)
        masina.mem[self.jump_ins] = self.counter_adr     ############
        
class IfElse(Syntax):
     def __init__(self, ntest, nbody):
        self.test = ntest
        self.body = nbody

     def generate(self):
        self.test.generate()
        masina.poke(self.INSTRUCTION_JUMPIFFALSE)
        self.jumpfalse_ins = self.counter_adr
        self.counter_adr +=1
        self.bodytrue.generate()
        if self.bodytrue == null:
            masina.mem[self.jumpfalse_ins] = self.counter_adr
        else:
            masina.poke(self.INSTRUCTION_JUMP)
            self.jump_ins =self.counter_adr
            self.counter_adr += 1
            masina.mem[self.jumpfalse_ins] = self.counter_adr
            self.bodyfalse.generate()
            masina.mem[self.jump_ins] = self.counter_adr



class Subroutine(Syntax):
     def __init__(self, ntest, nbody):
        self.test = ntest
        self.body = nbody
        self.bodyadr = 0
        

     def generate(self):
        masina.poke(self.INSTRUCTION_JUMP)
        self.counter_adr +=1
        self.bodyadr = self.counter_adr
        self.body.generate()
        masina.poke(self.INSTRUCTION_RETURN)
        masina.mem[self.bodyadr-1] = self.counter_adr

class Call(Syntax):
     def __init__(self, nname):
        self.name = nname
        
     def generate(self):
        masina.poke(self.INSTRUCTION_CALL)
        print('v slovniku mam ', name)
        masina.poke(subroutines [name].bodyadr)

        
class Expression:
    
    def Evaluate(self):
        return 0
    
class Const(Syntax):
    
    def __init__(self, nValue):
        self.Value = nValue
        
    def Evaluate(self):
        self.poke(self.INSTRUCTION_PUSH)
        self.poke(self.Value)

    
class BinaryOperation(Syntax):
    
    def __init__(self, nL, nR):
        self.L = nL
        self.R = nR
        
class Add(BinaryOperation):
    
    def Evaluate(self):
        self.L.Evaluate()
        self.R.Evaluate()
        self.poke(self.INSTRUCTION_ADD)
    
class Sub(BinaryOperation):
    
    def Evaluate(self):
        self.L.Evaluate()
        self.R.Evaluate()
        self.poke(self.INSTRUCTION_SUB)
    
class Mul(BinaryOperation):
    
    def Evaluate(self):
        self.L.Evaluate()
        self.R.Evaluate()
        self.poke(self.INSTRUCTION_MUL)
    
class Div(BinaryOperation):
    
    def Evaluate(self):
        self.L.Evaluate()
        self.R.Evaluate()
        self.poke(self.INSTRUCTION_DIV)
                
class Command:
    
    def Execute(self):
        pass
    
class Commands(Command):
    
    def __init__(self, *arg):
        self.Items = list(arg)
        
    def Add(self, Item):
        self.Items.append(Item)
        
    def Execute(self):
        for Item in self.Items:
            Item.Execute()

class Repeat(Command):
    
    def __init__(self, nCount, nBody):
        self.Count = nCount
        self.Body = nBody
        
    def Execute(self):
        n = self.Count.Evaluate()
        while n > 0:
            self.Body.Execute()
            n -= 1

###############################################################################
###############################################################################

class TurtleCommand(Command):
    
    def __init__(self, nParam):
        self.Param = nParam
        
class FD(TurtleCommand):
    
    def Execute(self):
        k.dopredu(self.Param.Evaluate())
        
class LT(TurtleCommand):
    
    def Execute(self):
        k.vlavo(self.Param.Evaluate())
        
class RT(TurtleCommand):
    
    def Execute(self):
        k.vpravo(self.Param.Evaluate())
        

def get(event):
    inputt = event.widget.get()
    print (inputt)
    k.getInput(inputt)
    k.next()
    k.scan()
    print(k.token)
    event.widget.delete(0, 'end')
    k.interpret()

######################################################################################
######################################################################################
######################################################################################
######################################################################################
            
master = Tk()
 
w = Canvas(master, width=600, height=400)
w.pack()
 
e = Entry(master, width=60)
e.bind('<Return>', get)
e.pack()
e.focus_set()

# k = Korytnacka(w)
# k.zmaz()
# k.farba(0, 0, 255)

# k.getInput('opakuj 4 [ dopredu 100 vpravo 90 ]')
# #get()
# k.next()
# k.scan()
# k.interpret()
#k.compile()

#program = k.compile()

#program.Execute()

# program = Repeat(Const(4),Commands(FD(Const(100)), RT(Const(90))))
# ##
# program.Execute()

# k = Korytnacka(w)
# k.zmaz()
# k.farba(0, 0, 255)
# k.getInput("opakuj 4 [dp 100 vp 90]")
# k.next()
# k.scan()

# k.interpret()
# k.bod(25)
# k.x = 20
# k.y = 400
# k.kresli("dld*ppd*ld", 60, 99, 0.5)



# mainloop()

# expression = Add(Const(1), Mul(Const(2), Const(3)))
# print(expression.Evaluate())


############# uloha2 #############
##################################

m = Machine1(w)
m.turtle.x = 50
m.mem = 100 * [0]


s = Call((FD(100),RT(90)))

#############################################################
##m.poke(m.INSTRUCTION_CALL)
##m.poke(9)
##m.poke(m.INSTRUCTION_CALL)
##m.poke(9)
##m.poke(m.INSTRUCTION_CALL)
##m.poke(9)
##m.poke(m.INSTRUCTION_CALL)
##m.poke(9)
##m.poke(0)
##
##m.poke(m.INSTRUCTION_PUSH)
##m.poke(100)
##m.poke(m.INSTRUCTION_FD)
##m.poke(m.INSTRUCTION_PUSH)
##m.poke(90)
##m.poke(m.INSTRUCTION_RT)
##m.poke(m.INSTRUCTION_RETURN)
################################################################

##s.generate(m)
##m.mem[0] = m.INSTRUCTION_SET
##m.mem[1] = 99
##m.mem[2] = 4
##m.mem[3] = m.INSTRUCTION_FD
##m.mem[4] = 100
##m.mem[5] = m.INSTRUCTION_RT
##m.mem[6] = 90
##m.mem[7] = m.INSTRUCTION_LOOP
##m.mem[8] = 99
##m.mem[9] = 3
##m.mem[10] = 0
##
##m.run()

############# uloha3 #############
##################################

# m.adr = 0
# m.poke(m.INSTRUCTION_SET)
# m.poke(98)
# m.poke(10)
# outer = m.adr
# m.poke(m.INSTRUCTION_SET)
# m.poke(99)
# m.poke(4)
# body = m.adr
# m.poke(m.INSTRUCTION_FD)
# m.poke(50)
# m.poke(m.INSTRUCTION_RT)
# m.poke(90)
# m.poke(m.INSTRUCTION_LOOP)
# m.poke(99)
# m.poke(body)

# m.poke(m.INSTRUCTION_RT)
# m.poke(90)
# m.poke(m.INSTRUCTION_FD)
# m.poke(51)
# m.poke(m.INSTRUCTION_LT)
# m.poke(90)
# m.poke(m.INSTRUCTION_LOOP)
# m.poke(98)
# m.poke(outer)


##m.run()

###################################
###################################

mainloop()
