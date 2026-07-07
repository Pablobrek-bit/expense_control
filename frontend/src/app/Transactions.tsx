import React, { useEffect, useState } from 'react';
import { transactionService } from '../services/transactions';
import { personService } from '../services/persons';
import type { Transaction, Person, PaginatedResponse } from '../types';
import { Loader2, Plus, Trash2, ArrowDownCircle, ArrowUpCircle } from 'lucide-react';

export default function Transactions() {
  const [data, setData] = useState<PaginatedResponse<Transaction> | null>(null);
  const [persons, setPersons] = useState<Person[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Estados do Modal
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newDesc, setNewDesc] = useState('');
  const [newValue, setNewValue] = useState<number | ''>('');
  const [newType, setNewType] = useState<'Income' | 'Expense'>('Expense');
  const [newPersonId, setNewPersonId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [uxWarning, setUxWarning] = useState('');

  const loadData = async () => {
    try {
      setLoading(true);
      const [transRes, personsRes] = await Promise.all([
        transactionService.getAll(1, 50),
        personService.getAll(1, 100)
      ]);
      setData(transRes);
      setPersons(personsRes.items);
    } catch (err) {
      console.error(err);
      setError('Falha ao carregar os dados.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    setUxWarning('');
    if (newPersonId && newType === 'Income') {
      const selectedPerson = persons.find(p => p.id === newPersonId);
      if (selectedPerson && selectedPerson.age < 18) {
        setUxWarning('Atenção: A regra de negócio não permite cadastrar Receitas (Income) para menores de idade. O servidor recusará a requisição.');
      }
    }
  }, [newPersonId, newType, persons]);

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja excluir esta transação?')) return;
    try {
      await transactionService.delete(id);
      loadData();
    } catch (err) {
      console.error(err);
      alert('Erro ao excluir transação.');
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newDesc || !newValue || !newType || !newPersonId) return;

    setIsSubmitting(true);
    try {
      await transactionService.create({
        description: newDesc,
        value: Number(newValue),
        type: newType,
        personId: newPersonId
      });
      setIsModalOpen(false);
      
      setNewDesc('');
      setNewValue('');
      setNewType('Expense');
      setNewPersonId('');
      
      loadData();
    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.detail || err.response?.data?.title || 'Erro ao criar transação.';
      alert(`Falha: ${errorMessage}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Transações</h2>
        <button
          onClick={() => setIsModalOpen(true)}
          className="flex items-center space-x-2 bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          <Plus size={20} />
          <span>Nova Transação</span>
        </button>
      </div>

      {error && (
        <div className="bg-red-50 text-red-600 p-4 rounded-lg border border-red-200">
          {error}
        </div>
      )}

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead className="text-xs text-gray-500 uppercase bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-6 py-4 font-semibold">Descrição</th>
                <th className="px-6 py-4 font-semibold">Pessoa</th>
                <th className="px-6 py-4 font-semibold">Tipo</th>
                <th className="px-6 py-4 font-semibold">Valor</th>
                <th className="px-6 py-4 font-semibold text-right">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center">
                    <Loader2 className="animate-spin text-emerald-500 mx-auto" size={32} />
                  </td>
                </tr>
              ) : data?.items.map((trans) => (
                <tr key={trans.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 font-medium text-gray-900">{trans.description}</td>
                  <td className="px-6 py-4 text-gray-600">{trans.personName}</td>
                  <td className="px-6 py-4">
                    {trans.type === 'Income' ? (
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-emerald-100 text-emerald-800">
                        <ArrowUpCircle size={14} className="mr-1" /> Receita
                      </span>
                    ) : (
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        <ArrowDownCircle size={14} className="mr-1" /> Despesa
                      </span>
                    )}
                  </td>
                  <td className={`px-6 py-4 font-semibold ${trans.type === 'Income' ? 'text-emerald-600' : 'text-red-600'}`}>
                    {formatCurrency(trans.value)}
                  </td>
                  <td className="px-6 py-4 text-right">
                    <button
                      onClick={() => handleDelete(trans.id)}
                      className="text-red-500 hover:text-red-700 p-2 rounded-full hover:bg-red-50 transition-colors inline-flex"
                      title="Excluir transação"
                    >
                      <Trash2 size={18} />
                    </button>
                  </td>
                </tr>
              ))}
              
              {!loading && data?.items.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                    Nenhuma transação cadastrada ainda.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg w-full max-w-md overflow-hidden flex flex-col max-h-[90vh]">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-white sticky top-0">
              <h3 className="text-lg font-bold text-gray-800">Nova Transação</h3>
              <button onClick={() => setIsModalOpen(false)} className="text-gray-400 hover:text-gray-600 text-2xl leading-none">&times;</button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 space-y-4 overflow-y-auto">
              
              {persons.length === 0 && (
                <div className="bg-orange-50 text-orange-700 p-3 rounded-lg text-sm mb-4">
                  Você precisa cadastrar uma pessoa primeiro antes de criar transações.
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Pessoa</label>
                <select
                  required
                  value={newPersonId}
                  onChange={(e) => setNewPersonId(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 outline-none transition-all bg-white"
                >
                  <option value="" disabled>Selecione uma pessoa</option>
                  {persons.map(p => (
                    <option key={p.id} value={p.id}>{p.name} ({p.age} anos)</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de Transação</label>
                <div className="grid grid-cols-2 gap-3">
                  <label className={`border rounded-lg p-3 flex items-center justify-center cursor-pointer transition-all ${
                    newType === 'Expense' ? 'bg-red-50 border-red-200 text-red-700 font-medium' : 'border-gray-200 text-gray-600 hover:bg-gray-50'
                  }`}>
                    <input type="radio" name="type" value="Expense" className="hidden" 
                           checked={newType === 'Expense'} onChange={() => setNewType('Expense')} />
                    <ArrowDownCircle size={18} className="mr-2" /> Despesa
                  </label>
                  <label className={`border rounded-lg p-3 flex items-center justify-center cursor-pointer transition-all ${
                    newType === 'Income' ? 'bg-emerald-50 border-emerald-200 text-emerald-700 font-medium' : 'border-gray-200 text-gray-600 hover:bg-gray-50'
                  }`}>
                    <input type="radio" name="type" value="Income" className="hidden" 
                           checked={newType === 'Income'} onChange={() => setNewType('Income')} />
                    <ArrowUpCircle size={18} className="mr-2" /> Receita
                  </label>
                </div>
              </div>

              {uxWarning && (
                <div className="bg-orange-50 text-orange-700 p-3 rounded-lg text-sm font-medium border border-orange-200">
                  {uxWarning}
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descrição</label>
                <input
                  type="text"
                  required
                  value={newDesc}
                  onChange={(e) => setNewDesc(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 outline-none transition-all"
                  placeholder="Ex: Conta de Luz, Salário"
                />
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Valor</label>
                <div className="relative">
                  <span className="absolute left-4 top-2 text-gray-500">R$</span>
                  <input
                    type="number"
                    required
                    min="0.01"
                    step="0.01"
                    value={newValue}
                    onChange={(e) => setNewValue(e.target.value ? Number(e.target.value) : '')}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 outline-none transition-all"
                    placeholder="0.00"
                  />
                </div>
              </div>

              <div className="pt-4 flex space-x-3 bg-white sticky bottom-0">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="flex-1 bg-gray-100 hover:bg-gray-200 text-gray-700 font-medium py-2 px-4 rounded-lg transition-colors"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={isSubmitting || persons.length === 0}
                  className="flex-1 bg-emerald-600 hover:bg-emerald-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex justify-center items-center disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {isSubmitting ? <Loader2 className="animate-spin" size={20} /> : 'Salvar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
