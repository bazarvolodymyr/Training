import { Component, ElementRef, inject, ViewChild, viewChild } from '@angular/core';
import { Question } from '../../data/interfaces/question.interface';
import { Answer } from '../../data/interfaces/answer.interface';
import { QuestionsService } from '../../data/services/questions.service';
import { FormArray, FormBuilder, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AnswerResult } from '../../data/interfaces/answer-result.interface';

@Component({
  selector: 'app-question-card',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './question-card.component.html',
  styleUrl: './question-card.component.scss'
})
export class QuestionCardComponent {
  questionService = inject(QuestionsService)
  fb = inject(FormBuilder)
  question!: Question
  answer: string = ""
  result: string = ""
  answerResult: AnswerResult | null = null
  anagrams:string[] = ["new", "know", "now", "knowledge"]
  fbAnagrams:string[] = this.anagrams
  questionForm = this.fb.group({
    //answer: [''],
    anagrams: this.fb.array(this.fbAnagrams.map(anagram => this.fb.control(anagram)))
  })

  @ViewChild('answerInput') answerInput!: ElementRef
  setFocus(){
    this.answerInput.nativeElement.focus()
  }
  constructor() {
      this.questionService.getQuestion()
        .subscribe(val => {
          this.question = val
        })
  }
  get anagramsControl(){
    return this.questionForm.get('anagrams') as FormArray
  }

  anagramClick(val: string){
     let anserArray = this.answer.split(' ')
    if(val.startsWith(anserArray.at(-1)!)){
      anserArray.pop()
      anserArray.push(val)
      this.answer = anserArray.join(" ") + ' '
    }else{
      this.answer += val + ' '
    }
    this.onAnswerChange()
  }

  onAnswerChange(){
    let anserArray = this.answer.split(' ')
    this.fbAnagrams = []
    this.anagrams.forEach(element => {
      if(!anserArray.includes(element)){
        this.fbAnagrams.push(element)
      }
    });
    this.questionForm.setControl("anagrams", this.fb.array(this.fbAnagrams.map(anagram => this.fb.control(anagram))))
  
  }
  
  splitName(name: string):{start:string, end:string}{
    let anserArray = this.answer.split(' ')
    let lastItem = anserArray.at(-1)
    if(lastItem && name.startsWith(lastItem)){
      return {start: lastItem, end:name.substring(lastItem.length)}
    }
    return {start: "", end:name}
  }
  
  sendAnswer(){
    //@ts-ignore
    this.questionService.sendAnswer({id: this.question.id, answer: this.answer})
      .subscribe(val => {
        this.answerResult = val
        if(this.answerResult){
          this.result = this.answerResult.exerciseStatus
        }
      })
  }
  
}
